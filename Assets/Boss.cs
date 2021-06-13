using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{

    public List<Transform> heads = new List<Transform>();
    public float lookSpd;
    public GameObject player;

    [Header("Stats")]
    public float currentHealth;
    public float maxHealth;
    public bool isDamageable;
    public float damageOnCollision = 20f;

    public enum bossStates { Stage1, Stage2, Stage3, Dead, Growing }
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
    public float NextBulletTime;
    public float bulletAmount;
    public BossBullet bulletPrefabGameObject;
    public float bulletDmg;
    public float bulletSpd;

    [Header("PawsAttack")]
    public float PawsTimerMax;
    public GameObject Paws;


    [Header("AttackTimers")]
    public float waitForAttackTimerDurMin;
    public float waitForAttackTimerDurMax;
    public float waitForAttackTimerDur;
    public float waitForAttackTimer;

    public Coroutine SalivaAttackCoro;
    public Coroutine PawsAttackCoro;
    public Coroutine LaserAttackCoro;

    public Animator bossAnim;

    public List<ClipVolume> musics = new List<ClipVolume>();

    public ClipVolume hurt;
    public ClipVolume startPhase;
    public ClipVolume headGrowing;
    public List<ClipVolume> endPhase = new List<ClipVolume>();

    public AudioSource mainSource;
    public AudioSource musicSource;
    void Start()
    {
        musicSource.clip = musics[0].clip;
        musicSource.Play();
        currentState = bossStates.Stage1;
        currentHealth = maxHealth;


        waitForAttackTimerDur = Random.Range(waitForAttackTimerDurMin, waitForAttackTimerDurMax);
        waitForAttackTimer = waitForAttackTimerDur;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != bossStates.Dead && currentState != bossStates.Growing)
        {
            if (PawsAttackCoro == null)
                BodyLooksAtPlayer();

            if (waitForAttackTimer >= 0)
            {
                waitForAttackTimer -= Time.deltaTime;
            }
            else if ((waitForAttackTimer <= 0 && SalivaAttackCoro == null) || (waitForAttackTimer <= 0 && LaserAttackCoro == null))
            {
                ChooseAttack(currentState);

                waitForAttackTimerDur = Random.Range(waitForAttackTimerDurMin, waitForAttackTimerDurMax);
                waitForAttackTimer = waitForAttackTimerDur;
            }
        }
        else if (currentState != bossStates.Growing)
        {
            transform.Translate(Vector3.down * Time.deltaTime, Space.World);
        }

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

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player") && currentState != bossStates.Dead)
        {
            Vector3 staggerDirection = -collision.GetContact(0).normal;
            Debug.DrawRay(collision.GetContact(0).point, -collision.GetContact(0).normal, Color.red, 5f);
            staggerDirection.y = 0;
            collision.transform.GetComponent<Player>().DealDamage(damageOnCollision, staggerDirection);
        }
    }

    private void ChooseAttack(bossStates stage)
    {
        switch (stage)
        {
            case bossStates.Stage1:
                if (SalivaAttackCoro == null)
                {
                    SalivaAttackCoro = StartCoroutine(SalivaRain());
                }
                break;
            case bossStates.Stage2:
                int i = Random.Range(0, 2);
                if (i == 0)
                {
                    if (SalivaAttackCoro == null)
                    {
                        SalivaAttackCoro = StartCoroutine(SalivaRain());
                    }
                    else if (LaserAttackCoro == null)
                    {
                        LaserAttackCoro = StartCoroutine(LaserAttack());
                    }
                }
                else if (i != 0)
                {
                    if (LaserAttackCoro == null)
                    {
                        LaserAttackCoro = StartCoroutine(LaserAttack());
                    }
                    else if (SalivaAttackCoro == null)
                    {
                        SalivaAttackCoro = StartCoroutine(SalivaRain());
                    }
                }
                break;
            case bossStates.Stage3:
                int j = Random.Range(0, 3);
                if (j == 0)
                {
                    if (SalivaAttackCoro == null)
                    {
                        SalivaAttackCoro = StartCoroutine(SalivaRain());
                    }
                    else if (LaserAttackCoro == null)
                    {
                        LaserAttackCoro = StartCoroutine(LaserAttack());
                    }
                    else if (PawsAttackCoro == null)
                    {
                        PawsAttackCoro = StartCoroutine(PawsAttack());
                    }
                }
                else if (j == 1)
                {
                    if (LaserAttackCoro == null)
                    {
                        LaserAttackCoro = StartCoroutine(LaserAttack());
                    }
                    else if (SalivaAttackCoro == null)
                    {
                        SalivaAttackCoro = StartCoroutine(SalivaRain());
                    }
                    else if (PawsAttackCoro == null)
                    {
                        PawsAttackCoro = StartCoroutine(PawsAttack());
                    }
                }
                else
                {
                    if (PawsAttackCoro == null)
                    {
                        PawsAttackCoro = StartCoroutine(PawsAttack());
                    }
                    else if (SalivaAttackCoro == null)
                    {
                        SalivaAttackCoro = StartCoroutine(SalivaRain());
                    }
                    else if (LaserAttackCoro == null)
                    {
                        LaserAttackCoro = StartCoroutine(LaserAttack());
                    }
                }
                break;
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
        SalivaAttackCoro = null;
        // currentState = bossStates.Idle;
    }
    IEnumerator PawsAttack()
    {
        float pawsTimer = 0;
        Paws.SetActive(true);
        while (pawsTimer <= PawsTimerMax)
        {
            pawsTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        PawsAttackCoro = null;
        Paws.SetActive(false);
    }
    IEnumerator LaserAttack()
    {
        int bulletsSpawned = 0;
        while (bulletsSpawned <= bulletAmount)
        {
            bulletsSpawned++;
            BossBullet bullet = Instantiate(bulletPrefabGameObject, heads[1].transform.position, Quaternion.identity);
            bullet._damage = bulletDmg;
            bullet._movement = (player.transform.position - heads[1].transform.position).normalized * bulletSpd;

            yield return new WaitForSeconds(NextBulletTime);
        }

        LaserAttackCoro = null;
    }
    Vector3 RandompPointOnUnityCircle(float radius)
    {
        Vector3 RdmSpherePos = (Random.onUnitSphere * radius) + transform.position;
        return new Vector3(RdmSpherePos.x, RdmSpherePos.y + 20f, RdmSpherePos.z);
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

    void BodyLooksAtPlayer()
    {
        float step = lookSpd * Time.deltaTime;
        var targetRot = Quaternion.LookRotation(transform.position - player.transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, step);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    public GameObject Trophee;

    public void DealDmg(float dmg)
    {
        if (!Invulnerable)
        {
            hurt.Play(mainSource);
            bossAnim.SetTrigger("hit");
            currentHealth = currentHealth - dmg;
            if (currentHealth / maxHealth <= 0.00f)
            {
                musicSource.Stop();
                currentState = bossStates.Dead;
                bossAnim.SetBool("dead", true);
                FindObjectOfType<Spear>().DisableSpear();
                VoicelinesManager.onBossKilled?.Invoke();
                Trophee.SetActive(true);
            }
            else if (currentHealth / maxHealth <= 0.33f && currentState != bossStates.Stage3)
            {

                StartCoroutine(growHead(2));
            }
            else if (currentHealth / maxHealth <= 0.66f && currentState != bossStates.Stage2 && currentState != bossStates.Stage3)
            {
                StartCoroutine(growHead(1));
            }
        }

    }


    public float growthSpd;
    public GameObject protectionDome;
    public bool Invulnerable;

    IEnumerator growHead(int number)
    {

        currentState = bossStates.Growing;
        protectionDome.SetActive(true);
        headGrowing.Play(mainSource);
        endPhase[number - 1].Play(mainSource);

        FindObjectOfType<Spear>().DisableSpear();


        musicSource.Stop();


        float t = 0;
        while (t <= 1)
        {
            heads[number].transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            Debug.Log(heads[number].transform.localScale);


            t += Time.deltaTime * growthSpd;
            yield return new WaitForEndOfFrame();
        }
        VoicelinesManager.onBossNewPhase?.Invoke();
        startPhase.Play(mainSource);
        yield return new WaitForSeconds(0.5f);


        if (number == 2)
        {
            lookSpd *= 5f;
            currentState = bossStates.Stage3;
            musicSource.clip = musics[2].clip;
            musicSource.Play();
        }
        else if (number == 1)
        {
            musicSource.clip = musics[1].clip;
            musicSource.Play();
            currentState = bossStates.Stage2;
        }
        protectionDome.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, circleRadius);
    }
}
