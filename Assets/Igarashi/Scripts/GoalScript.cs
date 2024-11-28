using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    private Gamemanager gamemanager;

    void Start()
    {
        // シーン内の GameManager を探す
        gamemanager = FindObjectOfType<Gamemanager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Shuyaku がゴールに触れたとき
        if (collision.CompareTag("Player"))
        {
            gamemanager.LoadAmetalkclubScreen();
        }
    }
}
