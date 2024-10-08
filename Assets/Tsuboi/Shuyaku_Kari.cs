using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuyaku_Kari : MonoBehaviour
{
    // 移動速度とダッシュ速度（Inspectorで調整可能）
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;

    //敵を踏んだ後のジャンプ力
    public float stompJumpForce = 10f;

    //無敵時間の長さ
    public float invincibilityTime = 2f;

    // Rigidbody2Dの参照
    private Rigidbody2D rb;

    // 地上にいるかの判定
    private bool isDashing = false;

    //ヒット回数を記録
    private int hitCount = 0;

    //主役がやられたかどうか
    private bool isDead = false;
   
    //無敵状態かどうか
    private bool isInvincible = false;

    private Animator anim = null;

    // ジャンプに関連する変数
    [SerializeField] GameObject groundCheckPos;
    [SerializeField] float fallMultiplier; // 落ちるときの速度の乗数
    [SerializeField] float jumpMultiplier; // ジャンプして上がるときの速度の乗数
    [SerializeField] float jumpForce;      // ジャンプの力
    [SerializeField] float jumpTime;       // ジャンプしていられる時間
    [SerializeField] float checkRadius;    // 地面接地の取得範囲
    public LayerMask Ground;

    Vector2 vecGavity;
    float jumpCounter;

    bool isJumping;
    bool doubleJump;

    void Start()
    {
        // Rigidbody2DとAnimatorの取得
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        vecGavity = new Vector2(0, -Physics2D.gravity.y);
    }

    void Update()
    {
       
        // 横移動とダッシュ
        WalkAndDash();

        // ジャンプの処理
        JumpHandler();

        // キャラクターの落下速度を徐々に増加させる
        if (rb.velocity.y < 0)
        {
            rb.velocity -= vecGavity * fallMultiplier * Time.deltaTime;
        }

        // ボタンを押した時間によってジャンプの高さを変える処理
        if (rb.velocity.y > 0 && isJumping)
        {
            jumpCounter += Time.deltaTime;

            if (jumpCounter > jumpTime)
            {
                isJumping = false;
            }

            float t = jumpCounter / jumpTime;
            float currentJumpM = jumpMultiplier;

            // ジャンプの半分に達したら徐々に速度を落とす
            if (t > 0.5f)
            {
                currentJumpM = jumpMultiplier * (1 - t);
            }

            rb.velocity += vecGavity * currentJumpM * Time.deltaTime;
        }
    }

    #region//歩行とダッシュの詳細
    // 歩行とダッシュの処理
    private void WalkAndDash()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // 右か左に移動している場合、常に「Run」アニメーションを再生
        if (moveInput != 0)
        {
            anim.SetBool("Run", true);
        }
        else
        {
            anim.SetBool("Run", false);
        }

        // ダッシュ中の処理
        if (Input.GetKey(KeyCode.Space) && (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)))
        {
            rb.velocity = new Vector2(dashSpeed, rb.velocity.y);
            transform.localScale = new Vector3(1, 1, 1); // キャラクターの向き
        }
        else if (Input.GetKey(KeyCode.Space) && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)))
        {
            rb.velocity = new Vector2(-dashSpeed, rb.velocity.y);
            transform.localScale = new Vector3(-1, 1, 1); // キャラクターの向き
        }
        else
        {
            // 通常の歩行処理
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

            // 左右に歩くときにキャラクターの向きを変更
            if (moveInput > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (moveInput < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
    #endregion

    #region//ジャンプと接地判定
    // ジャンプ処理
    private void JumpHandler()
    {
        if (IsGrounded() && (!Input.GetKey(KeyCode.Space) || !Input.GetKey(KeyCode.W) || !Input.GetKey(KeyCode.UpArrow)))
        {
           
            doubleJump = false;
            
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (IsGrounded() || doubleJump)
            {
                doubleJump = !doubleJump;
                isJumping = true;
                jumpCounter = 0;
                Jump();
                anim.SetTrigger("DoubleJump");
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            isJumping = false;
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    // 地面との接地判定
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheckPos.transform.position, checkRadius, Ground);
    }
    #endregion

    #region//接触したときに起きること
    // 敵との接触を管理する処理
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 contactPoint = collision.contacts[0].point;
            Vector2 center = collision.collider.bounds.center;

            // 無敵状態でない場合のみ処理する
            if (!isInvincible)
            {
                // 接触地点が敵の中心より上の場合は「踏んだ」とみなす
                if (contactPoint.y > center.y)
                {
                    StompOnEnemy(); // 踏んだ場合の処理
                }
                else
                {
                    HitByEnemy(); // それ以外はヒットとして処理
                }
            }
        }
    }

    // 敵を踏んだ場合の処理
    private void StompOnEnemy()
    {
        rb.velocity = new Vector2(rb.velocity.x, stompJumpForce); // 少しジャンプさせる
        anim.SetTrigger("Jump"); // ジャンプアニメーション再生
    }

    // 敵にヒットされた場合の処理
    private void HitByEnemy()
    {
        hitCount++;

        if (hitCount >= 2)
        {
            // 2回ヒットされた場合、プレイヤーを消す
            anim.SetTrigger("Hit"); // ヒットアニメーションを再生
            StartCoroutine(DestroyPlayerAfterAnimation()); // プレイヤーを削除
        }
        else
        {
            anim.SetTrigger("Hit"); // 1回目のヒット
            StartCoroutine(StartInvincibility()); // 無敵時間を開始
        }
    }

    // プレイヤーを倒した後に削除する処理
    private IEnumerator DestroyPlayerAfterAnimation()
    {
        yield return new WaitForSeconds(1.0f); // ヒットアニメーションの終了待ち
        gameObject.SetActive(false); // プレイヤーを非表示にして削除
    }

    // 無敵時間を開始する処理
    private IEnumerator StartInvincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime); // 無敵時間を待つ
        isInvincible = false;
    }
    #endregion

}
