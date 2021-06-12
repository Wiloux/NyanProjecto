using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPaws : MonoBehaviour
{
    [SerializeField] private Boss boss;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player") && boss.currentState != Boss.bossStates.Dead)
        {
            Vector3 staggerDirection = -collision.GetContact(0).normal;
            Debug.DrawRay(collision.GetContact(0).point, -collision.GetContact(0).normal, Color.red, 5f);
            staggerDirection.y = 0;
            collision.transform.GetComponent<Player>().DealDamage(boss.damageOnCollision, staggerDirection);
        }
    }
}
