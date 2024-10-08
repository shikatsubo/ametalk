using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkull : MonoBehaviour
{
    public float waveAmplitude = 2f; // 波の幅（大きさ）
    public float waveFrequency = 1f; // 波の頻度
    public float moveSpeed = 2f;     // ボスの移動速度
    public float barrierRegenTime = 5f; // バリアの再生時間
    public float speedMultiplier = 2f; // 2回目の攻撃を受けたときの速度倍率
    public float returnDelay = 2f;     // 元の位置に戻るまでの待機時間

    public float minY = -2f;         // ボスが移動できる最小の高さ（y座標）
    public float maxY = 5f;          // ボスが移動できる最大の高さ（y座標）

    private bool hasBarrier = true;  // バリアを持っているかどうか
    private int stompCount = 0;      // 踏まれた回数
    private bool isDead = false;     // ボスが倒されたかどうか
    private float barrierTimer = 0f; // バリア再生タイマー
    private int attackCount = 0; // バリアがない状態での攻撃回数
    private bool isReturning = false; // 元の位置に戻っている状態か


    private Rigidbody2D rb;
    private Animator anim;
    private GameObject player;       // プレイヤーの参照

    private Vector3 startPosition;   // ボスの初期位置
    private Vector3 targetPosition;  // ボスの追尾先（プレイヤー）

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player"); // プレイヤーのタグ設定が必要
        startPosition = transform.position;
    }

    void Update()
    {
        if (isDead) return;

        // 波の軌道でプレイヤーを追尾
        if (player != null)
        {
            MoveInWavePattern();
        }

        // バリア再生のタイマー処理
        if (!hasBarrier)
        {
            barrierTimer += Time.deltaTime;
            if (barrierTimer >= barrierRegenTime)
            {
                RegenerateBarrier();
            }
        }
    }

    // 波の軌道でプレイヤーを追尾
    private void MoveInWavePattern()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Vector3 waveOffset = new Vector3(0, Mathf.Sin(Time.time * waveFrequency) * waveAmplitude, 0);

        //なんかあったらY座標に反映のところまで消してみる

        // 新しい速度を計算
        Vector3 newVelocity = direction * moveSpeed + waveOffset;

        // 新しい位置のY座標を制限する
        float newYPosition = Mathf.Clamp(transform.position.y + newVelocity.y * Time.deltaTime, minY, maxY);

        // 速度を制限されたy座標に反映
        rb.velocity = new Vector2(newVelocity.x, newYPosition - transform.position.y);

        float currentSpeed = moveSpeed;

        // バリアがなく、攻撃を2回受けた後は速度を速くする
        if (attackCount >= 2)
        {
            currentSpeed *= speedMultiplier;
        }
    }

    // プレイヤーに踏まれたときの処理
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isDead)
        {
            if (hasBarrier)
            {
                // バリアがある場合、バリアを解除する
                if (collision.transform.position.y > transform.position.y) // 上から踏まれた
                {
                    RemoveBarrier();
                }
            }
            else
            {
                // バリアがない場合、踏まれたらダメージ処理
                if (collision.transform.position.y > transform.position.y) // 上から踏まれた
                {
                    StompOnBoss();
                }
            }
        }
    }

    // バリアを解除する処理
    private void RemoveBarrier()
    {
        hasBarrier = false;
        barrierTimer = 0f; // タイマーをリセット
        anim.SetTrigger("Hit_Wall_2"); // バリアがはがれるアニメーションを再生
        anim.SetTrigger("Idle2_Skull"); // バリアなし状態に移行
    }

    // バリアを再生成する処理
    private void RegenerateBarrier()
    {
        hasBarrier = true;
        attackCount = 0; // 攻撃回数をリセット
        anim.SetTrigger("Hit_Wall_1"); // バリア生成アニメーションを再生
        anim.SetTrigger("Idle1_Skull"); // バリアあり状態に移行
    }

    // 踏まれた際の処理
    private void StompOnBoss()
    {
        stompCount++;

        if (stompCount >= 3)
        {
            // 3回目の踏みつけでボスが倒される
            anim.SetTrigger("Hit"); // ヒットアニメーションを再生
            isDead = true;
            StartCoroutine(DestroyBossAfterAnimation());
        }
        else
        {
            // バリアが無い状態で踏まれたら再びバリアを付与する
            anim.SetTrigger("Hit_Wall_1"); // ダメージ時のアニメーションを再生
            RegenerateBarrier(); // バリアを再生成する
        }
    }

    private void HitByPlayer()
    {
        attackCount++;

        if (attackCount == 1 || attackCount == 2)
        {
            // 1回目と2回目の攻撃で元の位置に戻る
            StartCoroutine(ReturnToStartPosition());
        }
        else if (attackCount >= 3)
        {
            // 3回目の攻撃で速度を通常に戻す
            speedMultiplier = 1f;
        }
    }

    private IEnumerator ReturnToStartPosition()
    {
        isReturning = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(returnDelay); // 追加：戻る前に待機時間を設定

        while (Vector2.Distance(transform.position, startPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = startPosition;
        isReturning = false;
    }



    // ボスを倒した後、アニメーションが終わったら削除する
    private IEnumerator DestroyBossAfterAnimation()
    {
        yield return new WaitForSeconds(1.0f); // アニメーションの時間分待つ
        gameObject.SetActive(false); // ボスを非表示にする（消す）
    }
}
