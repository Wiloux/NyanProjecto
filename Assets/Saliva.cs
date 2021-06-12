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

    void Start()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, mask))
        {
            spawnedMarker = Instantiate(marker, new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z), Quaternion.LookRotation(-hit.normal));
        }

    }

    // Update is called once per frame
    void Update()
    { }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            GameObject spawnedPoison = Instantiate(poison, spawnedMarker.transform.position, spawnedMarker.transform.rotation);
            AcidPond acidscript = spawnedPoison.GetComponent<AcidPond>();
            if (bossScript.currentState == Boss.bossStates.Stage3)
            {
                acidscript.timerMax = -1;
            }
            else
            {
                acidscript.timerMax = bossScript.acidTimer;
            }
            bossScript.acidPonds.Add(acidscript);
            Destroy(gameObject);
            Destroy(spawnedMarker);
        }

        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().DealDamage(damage);
        }
    }

}
