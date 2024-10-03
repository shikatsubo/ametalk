using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    [HideInInspector] public int nowHP;
    [HideInInspector] public int maxHP;

    private const int InitialHP = 2;
    private bool isDefeat = false;

    private void Start()
     {
        nowHP = InitialHP;
        maxHP = InitialHP;
     }

  public void Damaged (int damage)
   { if (isDefeat)
    return;

    nowHP -= damage;

    if (nowHP <= 0)
    {
      isDefeat = true;
    }
   }

}
