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
            //Debug.Log($"Touched {collision.transform.name}");
            collider.enabled = false;
            stopping = true;
            finalRot = transform.rotation;
            Invoke(nameof(Stop), timeWithoutCollider);
        }
    }

    private void Stop()
    {
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        //Debug.Log("Stopped");
    }

    public void ResetForLaunch()
    {
        stopping = false;
        collider.enabled = false;
        rb.useGravity = true;
        Invoke(nameof(EnableCollider), timeWithoutCollider);
    }

    private void EnableCollider()
    {
        collider.enabled = true;
    }
}
