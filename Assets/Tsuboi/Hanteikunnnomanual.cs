using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hanteikunnnomanual : MonoBehaviour
{
    private string JimenTag = "Jimen";
    private bool isJimen = false;
    private bool isJimenEnter, isJimenStay, isJimenExit;

    //接地判定を返すメソッド
    //物理判定の更新毎に呼ぶ必要がある
    public bool IsJimen()
    {
        if (isJimenEnter || isJimenStay)
        {
            isJimen = true;
        }
        else if (isJimenExit)
        {
            isJimen = false;
        }

        isJimenEnter = false;
        isJimenStay = false;
        isJimenExit = false;
        return isJimen;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == JimenTag)
        {
            isJimenEnter = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == JimenTag)
        {
            isJimenStay = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == JimenTag)
        { 
            isJimenExit = true;
        }
    }
}
