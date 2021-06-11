using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float timeBeforeStop = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Touched");
        Invoke(nameof(Stop), timeBeforeStop);
    }

    private void Stop()
    {
        rb.velocity = Vector3.zero;
        Debug.Log("Stopped");
    }
}
