using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BossBullet : MonoBehaviour
{

    [SerializeField] private UnityEngine.ParticleSystem.Particle _onHitParticle;
    public float _damage = 1;
    public GameObject _splash;

    public Vector3 _movement;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().velocity = _movement;
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {

            if (other.CompareTag("Player"))
            {
                GameObject _splashSpawned = Instantiate(_splash, transform.position, Quaternion.identity);
                Destroy(_splashSpawned, 1f);
                // Deal damage to boss
                Player player = other.GetComponent<Player>();
                if (player == null) player = other.GetComponentInParent<Player>();

                if (player != null) player.DealDamage(_damage, -(transform.position - player.transform.position));
                //Debug.Log("damage");
            }

            if (!other.CompareTag("Boss") && other.name != "Paws")
            {
                GameObject _splashSpawned = Instantiate(_splash, transform.position, Quaternion.identity);
                Destroy(_splashSpawned, 1f);
                Destroy(gameObject);
            }


        }
    }
}
