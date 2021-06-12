using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    private Rigidbody rb;
    public new Collider collider;
    [SerializeField] private float timeWithoutCollider = 1f;
    bool stopping;
    Quaternion finalRot;

    [HideInInspector] public bool linkedToBoss = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(rb.velocity != Vector3.zero)
        {
            transform.LookAt(transform.position + rb.velocity);
        }
        if (stopping)
        {
            transform.rotation = finalRot;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!stopping)
        {
            if (collision.transform.CompareTag("Boss"))
            {
                linkedToBoss = true;
            }

            //Debug.Log($"Touched {collision.transform.name}");
            collider.enabled = false;
            stopping = true;
            finalRot = transform.rotation;
            Invoke(nameof(StopMovement), timeWithoutCollider);
        }
    }

    private void StopMovement()
    {
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        //Debug.Log("Stopped");
    }

    public void ResetForLaunch()
    {
        ResetBase();
        collider.enabled = false;
        rb.useGravity = true;
        rb.velocity = Vector3.zero;
        Invoke(nameof(EnableCollider), 0.05f);
    }

    private void EnableCollider()
    {
        collider.enabled = true;
    }

    public void DisableSpear()
    {
        ResetBase();
        gameObject.SetActive(false);
    }

    private void ResetBase()
    {
        linkedToBoss = false;
        stopping = false;
    }
}
