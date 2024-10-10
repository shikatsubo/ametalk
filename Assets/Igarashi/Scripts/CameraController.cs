using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メインカメラ制御クラス(Main Cameraにアタッチ)
/// </summary>
public class CameraController : MonoBehaviour
{
    //オブジェクト・コンポーネント
    public CameraMovingLimitter movingLimitter;

    //各種変数
    private Vector2 basePos; //基点座標
    private Rect limitQuad;//有効中のカメラ移動制限範囲

    //Start
    void Start ()
    {
        limitQuad = movingLimitter.GetSpriteRect ();
    }

    /// <summary>
    /// カメラの位置を動かす
    /// </summary>
    /// <param name="targetPos">座標</param>
    public void SetPosition (Vector2 targetPos)
    {
        basePos = targetPos;
    }

    //FixedUpdate
    private void FixedUpdate ()
    {
        //カメラ移動
        Vector3 pos = transform.localPosition;
        //主役の現在位置よりも少し右上を映すようにX・Y座標を補正
        pos.x = basePos.x + 2.5f; //X座標
        pos.y = basePos.y + 1.5f; //Y座標
        //Z座標は現在値（transform.localPosition）をそのまま使用

        //カメラ可動範囲を反映
        pos.x = Mathf.Clamp (pos.x, limitQuad.xMin, limitQuad.xMax);
        pos.y = Mathf.Clamp (pos.y, limitQuad.yMin, limitQuad.yMax);

        //計算後のカメラの座標を反映
        transform.localPosition = Vector3.Lerp (transform.localPosition, pos, 0.08f);
    }
}
