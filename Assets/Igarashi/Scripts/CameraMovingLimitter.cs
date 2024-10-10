using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovingLimitter : MonoBehaviour
{
    //オブジェクト・コンポーネント
    private SpriteRenderer spriteRenderer;

    //Start
    void Start ()
    {
        //参照取得
        spriteRenderer = GetComponent<SpriteRenderer>();
        //スプライトを透明にする
        spriteRenderer.color = Color.clear;
    }

    ///<summary>
    ///スプライトの上下左右の端の座標をRect型にして返す
    ///</summary>
    ///<>return>上下左右端座標のRect</returns>
    public Rect GetSpriteRect ()
    {
        Rect result = new Rect ();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer> ();
        Sprite sprite = spriteRenderer.sprite;

        float halfSizeX = sprite.bounds.extents.x;
        float halfSizeY = sprite.bounds.extents.y;

        Vector3 topLeft = new Vector3(-halfSizeX, halfSizeY, 0f);
        topLeft = spriteRenderer.transform.TransformPoint (topLeft);

        Vector3 bottomRight = new Vector3(halfSizeX, -halfSizeY, 0f);
        bottomRight = spriteRenderer.transform.TransformPoint(bottomRight);

        result.yMin = topLeft.y;
        result.xMax = bottomRight.x;
        result.yMax = bottomRight.y;

        return result;
    }
}
