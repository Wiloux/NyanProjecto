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

    public GameObject hackPanel;

    [HideInInspector] public bool linkedToBoss = false;

    public Player playerscript;

    [SerializeField] private LineRenderer link;

    private Transform playerHeart;
    private Transform bossHeart;

    [Header("Sounds")]
    [Space(20)]
    [SerializeField] private ClipVolume flyingAudio;
    [SerializeField] private ClipVolume plantingAudio;
    [SerializeField] private ClipVolume touchingBossAudio;
    private AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        link.enabled = false;
        playerHeart = GameObject.Find("PlayerHeart").transform;
        bossHeart = GameObject.Find("BossHeart").transform;
    }

    private void Update()
    {
        if (rb.velocity != Vector3.zero)
        {
            transform.LookAt(transform.position + rb.velocity);
        }
        if (stopping)
        {
            transform.rotation = finalRot;
            if(linkedToBoss) UpdateLink();
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!stopping)
        {
            audioSource.Stop();

            if (collision.transform.CompareTag("Boss"))
            {
                linkedToBoss = true;
                playerscript.mainAudioSource.PlayOneShot(playerscript.startLink.clip, playerscript.startLink.volume);
                SetLink();
                touchingBossAudio.Play(audioSource);
            }
            else if (collision.transform.GetComponent<Victory>())
            {
                collision.transform.GetComponent<Victory>().StartEnding();
                touchingBossAudio.Play(audioSource);
            }
            else plantingAudio.Play(audioSource);

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

        flyingAudio.Play(audioSource);
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
        link.SetPosition(0, linkedToBoss ? bossHeart.position : transform.position);
        link.SetPosition(1, playerHeart.position);
    }
}
