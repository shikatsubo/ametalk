using System.Collections;
using UnityEngine;

public class BossSkull : MonoBehaviour
{
    public float waveAmplitude = 2f;       // 波移動の振幅
    public float waveFrequency = 1f;       // 波移動の周波数
    public float moveSpeed = 2f;           // 通常の移動速度
    public float barrierRegenTime = 5f;    // バリア再生までの時間
    public float speedMultiplier = 2f;     // 2回目の攻撃を受けた後の速度倍率
    public float returnDelay = 0f;         // 元の位置に戻るまでの待機時間
    public float invincibleTime = 2f;      // 無敵時間

    public float minY = -2f;               // 移動可能な最小Y座標
    public float maxY = 5f;                // 移動可能な最大Y座標

    private bool hasBarrier = true;        // バリアを持っているかどうか
    private int stompCount = 0;            // ボスが踏まれた回数
    private bool isDead = false;           // ボスが倒されたかどうか
    private float barrierTimer = 0f;       // バリア再生のためのタイマー
    private int attackCount = 0;           // バリアなしでの攻撃回数
    private bool isReturning = false;      // ボスが元の位置に戻っている状態かどうか
    private bool isInvincible = false;     // 無敵状態かどうか

    private Rigidbody2D rb;
    private Animator anim;
    private GameObject player;

    private Vector3 startPosition;         // ボスの初期位置

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");  // プレイヤーのタグを持つオブジェクトを探す
        startPosition = transform.position;  // ボスの初期位置を保存
    }

    void Update()
    {
        if (isDead || isReturning) return;  // 死亡または戻り中の場合は操作を無効化

        if (player != null)
        {
            MoveInWavePattern();  // プレイヤーに向けて波状移動する
        }

        if (!hasBarrier)
        {
            barrierTimer += Time.deltaTime;  // バリア再生タイマーを進める
            if (barrierTimer >= barrierRegenTime)
            {
                RegenerateBarrier();  // バリアを再生
            }
        }
    }

    // ボスの波状移動
    private void MoveInWavePattern()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;  // プレイヤーへの方向
        Vector3 waveOffset = new Vector3(0, Mathf.Sin(Time.time * waveFrequency) * waveAmplitude, 0);  // 波移動の上下振れ幅

        float currentSpeed = moveSpeed;

        if (attackCount >= 2)
        {
            currentSpeed *= speedMultiplier;  // 2回目以降の攻撃で速度を速くする
        }

        // X方向の移動速度に波移動を加算し、Y座標の上限を設定
        rb.velocity = direction * currentSpeed + waveOffset;

        // Y座標の上限をmaxYに制限
        if (transform.position.y > maxY)
        {
            transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
        }
    }

    // プレイヤーとの衝突処理
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isDead)
        {
            if (hasBarrier)
            {
                if (collision.transform.position.y > transform.position.y)
                {
                    RemoveBarrier();  // バリアがある場合は取り除く
                }
            }
            else
            {
                if (!isInvincible && collision.transform.position.y > transform.position.y)
                {
                    StompOnBoss();  // 無敵状態でないときに踏まれた場合の処理
                }
            }
        }
    }

    // バリアを取り除く
    private void RemoveBarrier()
    {
        hasBarrier = false;
        barrierTimer = 0f;
        anim.SetTrigger("Hit_Wall_2");
        anim.SetTrigger("Idle2_Skull");
    }

    // バリアを再生する
    private void RegenerateBarrier()
    {
        hasBarrier = true;
        attackCount = 0;  // 攻撃回数をリセット
        anim.SetTrigger("Hit_Wall_1");
        anim.SetTrigger("Idle1_Skull");
    }

    // ボスが踏まれた際の処理
    private void StompOnBoss()
    {
        stompCount++;
        StartCoroutine(ActivateInvincibility());  // 無敵状態を一時的に有効化

        if (stompCount >= 5)
        {
            anim.SetTrigger("Hit");
            isDead = true;
            StartCoroutine(DestroyBossAfterAnimation());  // アニメーション後にボスを消す
        }
        else
        {
            anim.SetTrigger("Hit_Wall_1");
            RegenerateBarrier();  // バリアを再生する
        }
    }

    // 一時的に無敵状態を有効化するコルーチン
    private IEnumerator ActivateInvincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);  // 指定した秒数だけ無敵状態を維持
        isInvincible = false;
    }

    // プレイヤーから攻撃された場合の処理
    private void HitByPlayer()
    {
        attackCount++;

        if (attackCount == 1 || attackCount == 2)
        {
            StartCoroutine(ReturnToStartPosition());  // 1回目と2回目の攻撃で元の位置に戻る
        }
        else if (attackCount >= 3)
        {
            speedMultiplier = 1f;  // 3回目の攻撃で速度を通常に戻す
        }
    }

    // 元の位置に戻るコルーチン
    private IEnumerator ReturnToStartPosition()
    {
        isReturning = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(returnDelay);  // 戻る前の待機時間

        while (Vector2.Distance(transform.position, startPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = startPosition;
        isReturning = false;
    }

    // ボスが倒された後に削除するコルーチン
    private IEnumerator DestroyBossAfterAnimation()
    {
        yield return new WaitForSeconds(1.0f);  // アニメーションが完了するまで待機
        gameObject.SetActive(false);  // ボスを非表示にする
    }
}
