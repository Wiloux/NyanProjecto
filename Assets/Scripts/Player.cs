using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private new Collider collider;
    private PlayerController controller;

    [SerializeField] private float maxHealth;
    private float health;
    private bool dead;
    [SerializeField] private float constantHealRegen;
    [SerializeField] private float healthDrain;

    [Space(10)]
    [Header("Spear related")]
    [SerializeField] private Transform firePos;
    [SerializeField] private Spear spear;
    private Rigidbody spearRigidbody;
    [SerializeField] private float spearCooldownDuration;
    private float spearCooldown;
    private bool HasSpearCooldown => spearCooldown <= 0;
    [SerializeField] private float spearLaunchForce;
    [SerializeField] private float dashToSpearDuration = 0.2f;

    [Space(10)]
    [Header("Gun related")]
    [SerializeField] private GameObject gun;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float gunFireRate;
    private float nextTimeToFire;
    private bool gunShooting;

    [Space(10)]
    [SerializeField] private Animator animator;

    [Space(10)]
    [SerializeField] private float camZoomTransitionDuration = 0.5f;
    IEnumerator camZoomingCoroutine;
    private bool aiming;

    [Space(10)]
    [SerializeField] private float staggerDuration;
    [SerializeField] private float staggerForce;
    private float staggerTimer;
    public bool Staggered => staggerTimer > 0;
    [SerializeField] private float staggerInvulnerabilityDuration;
    [SerializeField] private float tpInvulnerabilityDuration;
    private float invulnerabilityTimer;
    private bool Invulnerable => invulnerabilityTimer > 0;

    private Camera cam;
    public Image healthImg;
    public GameObject bowTie;

    [Header("Audio")]
    [Space(20)]
    [SerializeField] private AudioSource mainAudioSource;
    [SerializeField] private AudioSource movementAudioSource;
    [Space(5)]
    [Header("Sounds")]
    [SerializeField] private ClipVolume spearThrowAudio;
    [SerializeField] private ClipVolume shootingAudio;
    [SerializeField] private ClipVolume stopShootingAudio;
    [SerializeField] private ClipVolume dashToSpearAudio;
    [SerializeField] private ClipVolume deathAudio;
    [SerializeField] private ClipVolume gettingHitAudio;
    [Space(5)]
    [Header("Movement sounds")]
    [SerializeField] private ClipVolume stepAudio;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;

        controller = GetComponent<PlayerController>();
        spear.gameObject.SetActive(false);

        collider = GetComponent<Collider>();
        Physics.IgnoreCollision(collider, spear.collider);

        spearRigidbody = spear.GetComponent<Rigidbody>();

        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        healthImg.fillAmount = health / maxHealth;

        if (!GameHandler.isPaused)
        {
            spear.hackPanel.SetActive(spear.linkedToBoss);
            bowTie.SetActive(!spear.linkedToBoss);

            if (GameHandler.enableControls && !Staggered)
            {
                bool spearLinkedToTheBoss = spear.gameObject.activeSelf && spear.linkedToBoss;

                if (KeyInput.GetDashKeyDown() && spear.gameObject.activeSelf)
                {
                    // Dash to spear
                    DashToSpear(() => GetInvulnerability(tpInvulnerabilityDuration));
                }

                #region Zoom
                else if (KeyInput.GetZoomKeyDown() && ((!spear.gameObject.activeSelf && HasSpearCooldown) || spearLinkedToTheBoss))
                {
                    // Start Zoom
                    ZoomCam();

                    // Set zooming
                    animator.SetBool("Aim", true);
                }
                else if (KeyInput.GetZoomKey() && aiming)
                {
                    // Launc Spear / Shoot with gun
                    #region Launch speat / shoot with gun
                    if (spearLinkedToTheBoss)
                    {
                        if (KeyInput.GetFireKey())
                        {
                            // Guns
                            if (Time.time >= nextTimeToFire)
                            {
                                gunShooting = true;

                                nextTimeToFire = Time.time + 1 / gunFireRate;
                                // Shoot
                                Bullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                                bullet.movement = cam.transform.forward * bulletSpeed;
                                shootingAudio.Play(mainAudioSource);
                            }
                        }
                        else if (KeyInput.GetFireKeyUp())
                        {
                            gunShooting = false;
                            stopShootingAudio.Play(mainAudioSource);
                        }
                    }
                    else
                    {
                        if (KeyInput.GetFireKeyDown() && !spear.gameObject.activeSelf)
                        {
                            // Launch Spear
                            if(HasSpearCooldown)
                            {
                                LaunchSpear(cam.transform.forward);
                            }

                            UnZoomCam();
                            animator.SetBool("Aim", false);
                        }
                    }
                    #endregion

                    // Animator
                    animator.SetBool("Spear", !spearLinkedToTheBoss);
                    // Make the player look in the direction of the cam
                    if (controller.camPivot.transform.forward != transform.forward)
                    {
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, controller.camPivot.transform.rotation, controller.rotationSpeed);
                    }
                }
                else if (KeyInput.GetZoomKeyUp() && aiming)
                {
                    if (gunShooting)
                    {
                        gunShooting = false;
                        stopShootingAudio.Play(mainAudioSource);
                    }

                    // End Zoom
                    UnZoomCam();

                    // Set not aiming
                    animator.SetBool("Aim", false);
                }
                #endregion

                #region Health gestion
                if (Input.GetKeyDown(KeyCode.X))
                {
                    health -= maxHealth / 5;
                }

                if (spear.linkedToBoss && spear.gameObject.activeSelf)
                {
                    health -= Time.deltaTime * healthDrain;
                }

                if (health < maxHealth)
                {
                    health += constantHealRegen * Time.deltaTime;
                    if (health > maxHealth) health = maxHealth;
                }

                if (health <= 0 && !dead)
                {
                    Die();
                }
                #endregion
            }
            else if (staggerTimer > 0)
            {
                staggerTimer -= Time.deltaTime;
                if (staggerTimer < 0) animator.SetBool("Stagger", false);
            }

            if (invulnerabilityTimer > 0) invulnerabilityTimer -= Time.deltaTime;
            if (spearCooldown > 0) spearCooldown -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }

        Debug.DrawRay(cam.transform.position, cam.transform.forward * 3, Color.red);
    }

    //private void OnGUI()
    //{
    //    GUIStyle style = new GUIStyle
    //    {
    //        fontSize = 40
    //    };
    //    style.normal.textColor = Color.white;

    //    string message = $"Health:{health} ";
    //    if (Staggered) message += $"Stagger:{staggerTimer} ";
    //    if (Invulnerable) message += $"invulnerable:{invulnerabilityTimer} ";
    //    GUILayout.Label(message, style);
    //}

    #region Spear Functions
    private void LaunchSpear(Vector3 direction)
    {
        spearCooldown = spearCooldownDuration;
        spear.transform.position = transform.position;
        spear.gameObject.SetActive(true);
        spear.ResetForLaunch();
        spearRigidbody.AddForce(direction * spearLaunchForce);

        spearThrowAudio.Play(mainAudioSource);
    }

    private void DashToSpear(System.Action onDashFinished)
    {
        GetInvulnerability(dashToSpearDuration);
        StartCoroutine(controller.LerpToPosition(spearRigidbody.position, dashToSpearDuration, () => { spear.DisableSpear(); onDashFinished?.Invoke(); }));

        dashToSpearAudio.Play(mainAudioSource);
    }
    #endregion

    #region Health methods
    public void DealDamage(float amount, Vector3? nullStaggerDirection = null)
    {
        if (health <= 0 || Invulnerable) return;

        health -= amount;
        if (health <= 0) Die();
        else if (nullStaggerDirection != null)
        {
            Vector3 staggerDir = (Vector3)nullStaggerDirection;
            staggerDir.y += 1;
            staggerDir.Normalize();

            animator.SetTrigger("Hurt");
            controller.rb.velocity = new Vector3(0, controller.rb.velocity.y, 0);
            controller.rb.AddForce(staggerDir * staggerForce);

            GetStaggered();
            GetInvulnerability(staggerInvulnerabilityDuration);

            gettingHitAudio.Play(mainAudioSource);
        }
    }

    private void Die()
    {
        if (!dead)
        {
            dead = true;
            controller.rb.velocity = new Vector3(0, controller.rb.velocity.y, 0);
            UnZoomCam();
            animator.SetTrigger("Dead");
            //GameHandler.SetPause(true);
            GameHandler.enableControls = false;
            GameHandler.instance.WaitForInput(2f, () =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
                GameHandler.enableControls = true;
                //GameHandler.SetPause(false);
            });

            deathAudio.Play(mainAudioSource);
        }
    }
    #endregion

    #region Cam Zoom functions
    private void ZoomCam()
    {
        if (camZoomingCoroutine != null) StopCoroutine(camZoomingCoroutine);
        camZoomingCoroutine = ZoomCamCoroutine();
        StartCoroutine(camZoomingCoroutine);

        aiming = true;
    }
    private IEnumerator ZoomCamCoroutine()
    {
        float timer = 0;
        while (timer < camZoomTransitionDuration)
        {
            if (cam.transform.localPosition != controller.camPivot.camZoomPosition.localPosition)
            {
                cam.transform.position = Vector3.Lerp(controller.camPivot.camPosition.position, controller.camPivot.camZoomPosition.position, timer / camZoomTransitionDuration);
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void UnZoomCam()
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        if (camZoomingCoroutine != null) StopCoroutine(camZoomingCoroutine);
        camZoomingCoroutine = UnZoomCamCoroutine();
        StartCoroutine(camZoomingCoroutine);

        aiming = false;
    }
    private IEnumerator UnZoomCamCoroutine()
    {
        float timer = 0;
        while (timer < camZoomTransitionDuration)
        {
            if (cam.transform.localPosition != controller.camPivot.camPosition.localPosition)
            {
                cam.transform.position = Vector3.Lerp(controller.camPivot.camZoomPosition.position, controller.camPivot.camPosition.position, timer / camZoomTransitionDuration);
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }
    #endregion

    #region Invulnerability/Stagger effects
    private void GetStaggered()
    {
        animator.SetBool("Walking", false);
        animator.SetBool("Running", false);
        animator.SetBool("Aim", false);
        animator.SetBool("Spear", false);

        animator.SetBool("Stagger", true);

        UnZoomCam();

        staggerTimer = staggerDuration;
    }
    private void GetInvulnerability(float duration)
    {
        invulnerabilityTimer = duration;
    }
    #endregion

    public void PlayStepSound()
    {
        stepAudio.Play(movementAudioSource);
    }

}
