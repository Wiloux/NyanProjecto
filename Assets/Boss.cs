using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{

    public List<Transform> heads = new List<Transform>();
    public float lookSpd;

    [Header("Stats")]
    public float currentHealth;
    public float maxHealth;
    public bool isDamageable;

    public enum bossStates { Stage1, Stage2, Stage3, Dead }
    [Header("States")]
    public bossStates currentState;

    [Header("SalivaDrop")]
    public float waitAmountBtwDrops;
    public float circleRadius;
    public float amountOfDropsMax;
    public GameObject salivaDrop;
    public List<AcidPond> acidPonds = new List<AcidPond>();
    public float acidTimer;

    [Header("LaserAttack")]
    public float laserTimerMax;
    public GameObject player;
    public GameObject laserGameObject;
    public LineRenderer lr;
    public float chargeWidth;
    public float DmgWidth;
    public float laserSpd;
    public LayerMask _layermask;


    void Start()
    {
        currentState = bossStates.Stage1;
        currentHealth = maxHealth;
        laserGameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == bossStates.Dead)
            HeadsLookAtPlayer();

        if (Input.GetKeyDown(KeyCode.E))
        {
            //   currentState = bossStates.SalivaRain;
            StartCoroutine(SalivaRain());
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            // currentState = bossStates.Laser;

            StartCoroutine(LaserAttack());
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            DealDmg(5);
        }
    }

    IEnumerator SalivaRain()
    {
        float amountOfDrops = 0;
        while (amountOfDrops <= amountOfDropsMax)
        {
            amountOfDrops++;
            Vector3 rpoc = RandompPointOnUnityCircle(circleRadius);
            //   rpoc += transform.position;
            GameObject spawnedSaliva = Instantiate(salivaDrop, rpoc, Quaternion.identity);
            spawnedSaliva.GetComponent<Saliva>().bossScript = this;
            yield return new WaitForSeconds(waitAmountBtwDrops);
        }
        // currentState = bossStates.Idle;
    }

    IEnumerator LaserAttack()
    {
        float laserTimer = 0;
        laserGameObject.SetActive(true);
        lr.startWidth = chargeWidth;
        lr.endWidth = chargeWidth;
        while (laserTimer <= laserTimerMax)
        {
            if (laserGameObject.activeInHierarchy)
            {
                RaycastHit hit;
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(transform.position, player.transform.position - lr.transform.position, out hit, Mathf.Infinity, _layermask))
                {
                    Vector3 hitPos = hit.point;
                }


                laserTimer += Time.deltaTime;
                if (laserTimer / laserTimerMax >= 0.9f)
                {
                    lr.startWidth = DmgWidth;
                    lr.endWidth = DmgWidth;
                    //DealDmg;
                }
                else
                {
                    Vector3 smoothedPosition = Vector3.Lerp(lr.GetPosition(1), hit.point - lr.transform.position, laserSpd);
                    lr.SetPosition(1, smoothedPosition);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        laserGameObject.SetActive(false);

    }
    Vector3 RandompPointOnUnityCircle(float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float x = Mathf.Sin(angle) * radius;
        float y = Mathf.Cos(angle) * radius;

        Vector3 RdmSpherePos = Random.onUnitSphere * radius;
        return new Vector3(RdmSpherePos.x, transform.position.y + 40f, RdmSpherePos.z);
    }

    void HeadsLookAtPlayer()
    {
        float step = lookSpd * Time.deltaTime;
        foreach (Transform head in heads)
        {
            Vector3 smoothedPosition = Vector3.Lerp(head.position, player.transform.position - head.transform.position, lookSpd);
            head.LookAt(smoothedPosition, Vector3.up);
            //var targetRot = Quaternion.LookRotation(player.transform.position - head.transform.position);
            //head.transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, step);
        }
    }

    void DealDmg(float dmg)
    {
        currentHealth = currentHealth - dmg;
        if (currentHealth / maxHealth <= 0.00f)
        {
            currentState = bossStates.Dead;
            if (laserGameObject.activeInHierarchy)
            {
                laserGameObject.SetActive(false);
            }
        }
        else if (currentHealth / maxHealth <= 0.33f)
        {
            heads[2].gameObject.SetActive(true);
            currentState = bossStates.Stage3;
            //foreach (AcidPond acid in acidPonds)
            //{
            //    Destroy(acid.gameObject);
            //}
            //acidPonds.Clear();
        }
        else if (currentHealth / maxHealth <= 0.66f)
        {
            heads[1].gameObject.SetActive(true);
            //foreach (AcidPond acid in acidPonds)
            //{
            //    Destroy(acid.gameObject);
            //}
            //acidPonds.Clear();
            currentState = bossStates.Stage2;
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, circleRadius);
    }
}
