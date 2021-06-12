using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float spearLaunchForce;
    [SerializeField] private float dashToSpearDuration = 0.2f;

    [Space(10)]
    [Header("Gun related")]
    [SerializeField] private GameObject gun;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float gunFireRate;
    private float nextTimeToFire;

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
    [SerializeField] private float invulnerabilityDuration;
    [SerializeField] private float tpInvulnerabilityDuration;
    private float invulnerabilityTimer;
    private bool Invulnerable => invulnerabilityTimer > 0;

    private Camera cam;

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
        if (!GameHandler.isPaused)
        {
            if (GameHandler.enableControls && !Staggered)
            {
                bool spearLinkedToTheBoss = spear.gameObject.activeSelf && spear.linkedToBoss;

                if (Input.GetKeyDown(KeyCode.E) && spear.gameObject.activeSelf)
                {
                    // Dash to spear
                    DashToSpear();
                }

                #region Zoom
                else if (Input.GetMouseButtonDown(1) && (!spear.gameObject.activeSelf || spearLinkedToTheBoss))
                {
                    // Start Zoom
                    ZoomCam();
                    
                    // Set zooming
                    animator.SetBool("Aim", true);
                }
                else if (Input.GetMouseButton(1) && aiming)
                {
                    // Launc Spear / Shoot with gun
                    #region Launch speat / shoot with gun
                    if (spearLinkedToTheBoss)
                    {
                        if (Input.GetMouseButton(0))
                        {
                            // Guns
                            if (Time.time >= nextTimeToFire)
                            {
                                nextTimeToFire = Time.time + 1 / gunFireRate;
                                // Shoot
                                Bullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                                bullet.movement = cam.transform.forward * bulletSpeed;
                            }
                        }
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0) && !spear.gameObject.activeSelf)
                        {
                            // Launch Spear
                            LaunchSpear(cam.transform.forward);

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
                else if (Input.GetMouseButtonUp(1) && aiming)
                {
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
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }

        Debug.DrawRay(cam.transform.position, cam.transform.forward * 3, Color.red);
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle
        {
            fontSize = 40
        };
        style.normal.textColor = Color.white;

        string message = $"Health:{health} ";
        if (Staggered) message += $"Stagger:{staggerTimer} ";
        if (Invulnerable) message += $"invulnerable:{invulnerabilityTimer} ";
        GUILayout.Label(message, style);
    }

    #region Spear Functions
    private void LaunchSpear(Vector3 direction)
    {
        spear.transform.position = transform.position;
        spear.gameObject.SetActive(true);
        spear.ResetForLaunch();
        spearRigidbody.AddForce(direction * spearLaunchForce);
    }

    private void DashToSpear()
    {
        StartCoroutine(controller.LerpToPosition(spearRigidbody.position, dashToSpearDuration, () => spear.DisableSpear()));
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
            GetInvulnerability();
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
    private void GetInvulnerability()
    {
        invulnerabilityTimer = invulnerabilityDuration;
    }
}
