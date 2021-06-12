using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saliva : MonoBehaviour
{

    public Boss bossScript;
    public LayerMask mask;
    public GameObject marker;
    public GameObject poison;
    GameObject spawnedMarker;

    [Space(10)]
    [SerializeField] private float damage;

    [Header("Sounds")]
    [Space(20)]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ClipVolume hittingGroundAudio;

    void Start()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, mask))
        {
            spawnedMarker = Instantiate(marker, new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z), Quaternion.LookRotation(-hit.normal));
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    { }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            GameObject spawnedPoison = Instantiate(poison, 
                spawnedMarker.transform.position, 
                spawnedMarker.transform.rotation);
            AcidPond acidscript = spawnedPoison.GetComponent<AcidPond>();
            acidscript.bossScript = bossScript;
            if (bossScript.currentState == Boss.bossStates.Stage3)
            {
                acidscript.timerMax = bossScript.acidTimer*2;
            }
            else
            {
                acidscript.timerMax = bossScript.acidTimer;
            }

            hittingGroundAudio.Play(audioSource);

            bossScript.acidPonds.Add(acidscript);

            GetComponentInChildren<Renderer>().enabled = false;
            Destroy(gameObject, 5f);
            Destroy(spawnedMarker);
        }

        if (other.CompareTag("Player"))
        {
            Vector3 staggerDir = other.bounds.center - transform.position;
            staggerDir.y = 0;

            other.GetComponent<Player>().DealDamage(damage, staggerDir);

            hittingGroundAudio.Play(audioSource);
        }
    }

}
