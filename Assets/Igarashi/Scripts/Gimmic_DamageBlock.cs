using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージギミック：ダメージ床
/// ブロックに乗った主役（またはトリガーに侵入した主役）にダメージを与える
/// </summary>
public class Gimmic_DamageBlock : MonoBehaviour
{
    //設定項目
        [Header ("1")]
        public int damage;

    //主役接触時処理（Trriger）
    private void OntriggerStay2D (Collider2D collision)  
    {
        string tag = collision.gameObject.tag;

        if (tag == "Shuyaku")
        {
            //主役にダメージを与える
            collision.gameObject.GetComponent<ActorController> ().Damaged (damage);
        }
    }
    //主役接触時処理（Collider）
    private void OnCollisionStay2D (Collision2D collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Shuyaku")
        {
            //主役にダメージを与える
            collision.gameObject.GetComponent<ActorController> ().Damaged (damage);
        } 
    }
}
