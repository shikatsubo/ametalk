using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_base : MonoBehaviour
{
    protected Rigidbody2D rigidbody2D;
    protected SpriteRenderer spriteRenderer;
    protected Transform actorTransform;

    [Header ("最大体力（初期体力）")]
    public int maxHP;
    [Header ("接触時の主役へのダメージ")]
    public int touchDamage;

    [HideInInspector] public int nowHP;
    [HideInInspector] public bool isVanishing;
    [HideInInspector] public bool isInvis;
    [HideInInspector] public bool rightFacing;

    public bool Damaged (int damage)
    {
        nowHP -= damage;

        if (nowHP <= 0)
        {
            StartCoroutine(DestroyEnemy());
             return true;
        }
        return false;
    }

    private IEnumerator DestroyEnemy()
    {
        gameObject.SetActive(false);
        yield return null;
    }

    public void SetFacingright (bool isRight)
    {
        if (!isRight)
        {
            spriteRenderer.flipX = false;
            rightFacing = false;
        }
        else
        {
            spriteRenderer.flipX = true;
            rightFacing = true;
        }
    }

    public void BodyAttack (GameObject shuyakuObj)
    {
        Shuyaku_Kari shuyakuCtrl = shuyakuObj.GetComponent<Shuyaku_Kari> ();
        if (shuyakuCtrl == null)
            return;

        shuyakuCtrl.Hit (touchDamage);    
    }
}
