using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmic_Jump : MonoBehaviour
{
    // 設定項目
    [Header ("ジャンプ力")]
    public float JumpPower;

    //各トリガー呼び出し処理
    //トリガー滞在時に呼び出し
    private void OnTriggerStay2D(Collider2D collision)
     {
       //接しているのが主役の設置判定オブジェクトでない場合は終了
       Hanteikun hanteikun = collision.gameObject.GetComponent<Hanteikun> ();
       if (hanteikun == null)
       return;

       //主役を移動させる
       var rigidbody2D = collision.gameObject.GetComponentInParent<Rigidbody2D> ();
       rigidbody2D.velocity += new Vector2 (0.0f, JumpPower); 
     }
}
