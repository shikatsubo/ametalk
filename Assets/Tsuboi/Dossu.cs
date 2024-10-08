using System.Collections;
using UnityEngine;

public class Dossu : MonoBehaviour
{
    public float blinkRangeX = 1f;
    public float blinkRangeY = 5f;
    public float fallSpeed = 10f;
    public float riseSpeed = 2f;
    public float riseDelay = 2f;
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    private Vector2 originalPosition;
    private bool isFalling = false;
    private bool isRising = false;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        originalPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // X軸の位置と回転の固定
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        if (!isFalling && !isRising)
        {
            float distanceToPlayerX = Mathf.Abs(transform.position.x - player.position.x);
            float distanceToPlayerY = Mathf.Abs(transform.position.y - player.position.y);

            if (distanceToPlayerX < blinkRangeX && distanceToPlayerY < blinkRangeY)
            {
                anim.SetTrigger("Blink_D");
                if (Mathf.Abs(transform.position.x - player.position.x) < 0.5f)
                {
                    StartFalling();
                }
            }
        }
    }

    private void StartFalling()
    {
        isFalling = true;
        // X軸固定と回転固定を維持しつつ、Y軸のみ解除
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 1;
        rb.velocity = new Vector2(0, -fallSpeed);
    }

    private void FixedUpdate()
    {
        // 回転固定を常に維持
        rb.constraints |= RigidbodyConstraints2D.FreezeRotation;

        if (isFalling)
        {
            float rayDistance = 0.5f;
            RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, groundLayer);
            RaycastHit2D playerHit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, playerLayer);

            if (groundHit.collider != null || playerHit.collider != null)
            {
                isFalling = false;
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;
                anim.SetTrigger("Bottom_D");

                // 落下完了後、Y軸の固定も追加
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

                StartCoroutine(RiseAfterDelay());
            }
        }
    }

    private IEnumerator RiseAfterDelay()
    {
        yield return new WaitForSeconds(riseDelay);

        isRising = true;
        // 上昇中はX軸のみ固定して回転も固定
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        while (Vector2.Distance(transform.position, originalPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, originalPosition, riseSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = originalPosition;
        isRising = false;

        // 元の位置に戻った後、位置と回転を再固定
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }
}
