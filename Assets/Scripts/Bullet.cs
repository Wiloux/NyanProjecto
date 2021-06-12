using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{

    [SerializeField] private UnityEngine.ParticleSystem.Particle onHitParticle;
    [SerializeField] private float damage = 1;

    public Vector3 movement;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().velocity = movement;
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            // Deal damage to boss
            Boss boss = other.GetComponent<Boss>();
            if (boss == null) boss = other.GetComponentInParent<Boss>();

            if (boss != null) boss.DealDmg(damage);
            //Debug.Log("damage");
        }

        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
