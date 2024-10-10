using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBodyAttack : MonoBehaviour
{
    private Enemy_base enemy_base;

    void Start()
    {
        enemy_base = GetComponentInParent<Enemy_base> ();

        var Coll_TouchArea = GetComponent<BoxCollider2D> ();
        var Coll_body = enemy_base.gameObject.GetComponent<BoxCollider2D> ();
        Coll_TouchArea.offset = Coll_body.offset;
        Coll_TouchArea.size =Coll_body.size;
        Coll_TouchArea.size *= 0.8f;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Player")
        {
            enemy_base.BodyAttack (collision.gameObject);
        }
    }
}
