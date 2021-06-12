using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private new Collider collider;
    private PlayerController controller;

    [SerializeField] private float maxHealth;
    private float health;
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
            bool spearLinkedToTheBoss = spear.gameObject.activeSelf && spear.linkedToBoss;
            if (spearLinkedToTheBoss)
            {
                if (Input.GetMouseButton(0))
                {
                    // Guns
                    if(Time.time >= nextTimeToFire)
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
                if (Input.GetMouseButtonDown(0))
                {
                    // Launch Spear
                    LaunchSpear(cam.transform.forward);
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                // Dash to spear
                DashToSpear();
            }

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

            if (health <= 0)
            {
                Die();
            }
            #endregion

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            }

            Debug.DrawRay(cam.transform.position, cam.transform.forward * 3, Color.red);
        }
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle
        {
            fontSize = 40
        };
        style.normal.textColor = Color.white;

        GUILayout.Label($"Health: {health}", style);
    }

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

    public void DealDamage(float amount)
    {
        health -= amount;
        if (health <= 0) Die();
    }

    private void Die()
    {
        GameHandler.instance.SetPause(true);
        GameHandler.instance.WaitForInput(2f, () =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1;
        });
    }
}
