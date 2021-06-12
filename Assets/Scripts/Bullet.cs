using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{

    [SerializeField] private UnityEngine.ParticleSystem.Particle onHitParticle;

    public Vector3 movement;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().velocity = movement;
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Boss"))
        {
            // Deal damage to boss
            //Debug.Log("damage");
        }

        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
