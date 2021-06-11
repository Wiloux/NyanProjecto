using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private new Collider collider;
    private PlayerController controller;

    [SerializeField] private Spear spear;
    private Rigidbody spearRigidbody;
    [SerializeField] private float spearLaunchForce;

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        spear.gameObject.SetActive(false);

        Collider spearCollider = spear.GetComponent<Collider>();
        if(spearCollider != null)
        {
            Physics.IgnoreCollision(collider, spearCollider);
        }

        spearRigidbody = spear.GetComponent<Rigidbody>();

        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LaunchSpear(cam.transform.forward);
        }
        else if(Input.GetMouseButtonDown(1) && spear.gameObject.activeSelf)
        {
            // Dash to spear
            DashToSpear();
        }
    }

    private void LaunchSpear(Vector3 direction)
    {
        spear.gameObject.SetActive(true);
        spearRigidbody.AddForce(direction * spearLaunchForce);
    }

    private void DashToSpear()
    {
        StartCoroutine(controller.LerpToPosition(spearRigidbody.position, 0.2f));
    }
}
