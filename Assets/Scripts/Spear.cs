using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    private Rigidbody rb;
    public new Collider collider;
    [SerializeField] private float timeWithoutCollider = 1f;
    [HideInInspector] public bool stopping;
    Quaternion finalRot;

    [HideInInspector] public bool linkedToBoss = false;

    [SerializeField] private LineRenderer link;

    private Transform playerHeart;
    private Transform bossHeart;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        link.enabled = false;
        playerHeart = GameObject.Find("PlayerHeart").transform;
        bossHeart = GameObject.Find("BossHeart").transform;
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
            if (linkedToBoss)
            {
                UpdateLink();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!stopping)
        {
            if (collision.transform.CompareTag("Boss"))
            {
                linkedToBoss = true;
                SetLink();
            }

            //Debug.Log($"Touched {collision.transform.name}");
            collider.enabled = false;
            stopping = true;
            finalRot = transform.rotation;
            Invoke(nameof(StopMovement), timeWithoutCollider);
        }
    }

    #region Stop/Reset/Enable..

    private void StopMovement()
    {
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        //Debug.Log("Stopped");
    }

    private void ResetBase()
    {
        linkedToBoss = false;
        stopping = false;
    }
    public void ResetForLaunch()
    {
        ResetBase();
        collider.enabled = false;
        rb.useGravity = true;
        rb.velocity = Vector3.zero;
        StopAllCoroutines();
        Invoke(nameof(EnableCollider), 0.05f);
        
        link.enabled = false;
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
    #endregion

    private void SetLink()
    {
        link.enabled = true;
        UpdateLink();
    }
    private void UpdateLink()
    {
        link.SetPosition(0, bossHeart.position);
        link.SetPosition(1, playerHeart.position);
    }
}
