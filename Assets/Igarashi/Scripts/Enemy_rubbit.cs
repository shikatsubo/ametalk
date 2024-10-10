using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_rubbit : MonoBehaviour
{
    [Header("移動速度")]
    public float movingSpeed;
    [Header("最大移動速度")]
    public float maxSpeed;
    [Header("移動条件")]
    public float awakeDistance;
    [Header("非移動時減速率")]
    public float brakeRatio;

    public Rigidbody2D rb;

    private bool isBreaking;
    private Transform shuyakuTransform;

    void Start()
   {
       rb = GetComponent<Rigidbody2D>();
       GameObject shuyaku = GameObject.FindWithTag("Player");
       if (shuyaku != null)
       {
           shuyakuTransform = shuyaku.transform;
       }
       else
       {
           Debug.LogError("Player タグの付いたオブジェクトが見つかりません。");
       }
   }  

    void Update()
    {
        float speed = 0.0f;
        Vector2 ePos = transform.position;
        Vector2 aPos = shuyakuTransform.position;

        if (Vector2.Distance(ePos, aPos) > awakeDistance)
        {
            isBreaking = true;
            return;
        }
        isBreaking = false;

        if (ePos.x > aPos.x)
        {
            speed = -movingSpeed;
            SetFacingRight(true);
        }
        else
        {
            speed = movingSpeed;
            SetFacingRight(true);
        }

        Vector2 vec = GetComponent<Rigidbody2D>().velocity;
        vec.x += speed * Time.deltaTime;

        if (vec.x > 0.0f)
            vec.x = Mathf.Clamp (vec.x, 0.0f, maxSpeed);
        else
            vec.x = Mathf.Clamp (vec.x, -maxSpeed,0.0f);

       GetComponent<Rigidbody2D>().velocity = vec;    
    }

    void FixedUpdate ()
    {
        if (isBreaking)
        {
            Vector2 vec = GetComponent<Rigidbody2D>().velocity;
            vec.x *= brakeRatio;
            GetComponent<Rigidbody2D>().velocity = vec;
        }
    }

    private void SetFacingRight(bool facingRight)
    {
        Vector3 localScale = transform.localScale;
        localScale.x = facingRight ? 1 : -1;
        transform.localScale = localScale;
    }
}
