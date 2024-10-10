using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージギミック：隠し壁
/// 主役が通ってほしくない場所に設置する
/// </summary>
public class Gimmic_Hidden : MonoBehaviour
{
    // Start
    void Start()
    {
        GetComponent<SpriteRenderer> ().color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
