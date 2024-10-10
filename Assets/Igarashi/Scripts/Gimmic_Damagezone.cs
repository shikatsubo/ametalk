using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージギミック：ダメージ床
/// ブロックに乗った主役（またはトリガーに侵入した主役）にダメージを与える
/// </summary>
public class Gimmic_Damagezone : MonoBehaviour
{
    //設定項目
        [Header ("ダメージ量")]
        public int damageAmount = 2; //ダメージ量を２にする
    //主役接触時処理（Trriger）
    private void OntriggerStay2D (Collider2D collision)  
    {
        string tag = collision.gameObject.tag;

        if (tag == "Shuyaku")
        {
            //主役にダメージを与える
            Shuyaku_Kari shuyaku = collision.gameObject.GetComponent<Shuyaku_Kari>();
            shuyaku.Hit(damageAmount);
        }
    }
    //主役接触時処理（Collider）
    private void OnCollisionStay2D (Collision2D collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Shuyaku")
        {
            //主役にダメージを与える
            Shuyaku_Kari shuyaku = collision.gameObject.GetComponent<Shuyaku_Kari>();
            shuyaku.Hit(damageAmount);
        }
    }
}