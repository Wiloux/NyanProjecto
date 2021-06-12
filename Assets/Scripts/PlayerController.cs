using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Player player;

    [HideInInspector] public Rigidbody rb;

    [Header("Speeds")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeed;
    public float rotationSpeed;

    [Space(10)]
    [Header("Cam")]
    public CamPivot camPivot;
    [SerializeField] private Vector2 camRotatorVerticalAngleMinMax;

    //private bool isGrounded;
    [Space(10)]
    //[Header("Ground Check")]
    //[SerializeField] private LayerMask groundMask;
    //[SerializeField] private float groundRange;
    [SerializeField] private Transform groundCheck; // Just used to locate player's feets
    //[SerializeField] private float jumpForce;

    [Space(10)]
    [SerializeField] private float horizontalMouseSensivity;
    [SerializeField] private float verticalMouseSensivity;

    [SerializeField] private Animator animator;


    bool movingToSpear;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameHandler.isPaused && GameHandler.enableControls && !player.Staggered)
        {
            if (!movingToSpear)
            {
                // Get input
                Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

                if (input == Vector2.zero)
                {
                    animator.SetBool("Walking", false);
                    rb.velocity = new Vector3(0, rb.velocity.y, 0);
                }
                else
                {
                    // Choose current speed
                    bool running = KeyInput.GetSprintKey();
                    float currentSpeed = running ? runSpeed : moveSpeed;

                    // Animator
                    animator.SetBool("Running", running);
                    animator.SetBool("Walking", !running);

                    // Get movement direction
                    input.Normalize();
                    Vector3 moveDirection = camPivot.transform.forward * input.y + input.x * camPivot.transform.right;
                    moveDirection.y = 0;

                    // Get the movement vector
                    Vector3 movement = moveDirection * currentSpeed;
                    movement.y = rb.velocity.y;

                    // Set the velocity of the player
                    rb.velocity = movement;

                    // Rotate the player depending of the movement direction
                    Quaternion targetRot = Quaternion.LookRotation(moveDirection, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                }

                //Debug.DrawRay(transform.position, transform.forward, Color.green);
                //Debug.DrawRay(transform.position, transform.right, Color.red);

                // Rotate the cam pivot on the Y axis depending on the MouseX axis
                float rotX = KeyInput.GetHorizontalCamAxis() * horizontalMouseSensivity * Mathf.Deg2Rad;
                camPivot.transform.Rotate(0, rotX, 0);

                // Rotate the cam pivot on the X axis depending on the MouseY axis
                float rotY = KeyInput.GetVerticalCamAxis() * verticalMouseSensivity * Mathf.Deg2Rad;
                camPivot.transform.Rotate(-rotY, 0, 0);
                // Lock Camera pivot between two max angle on the X axis
                if (camPivot.transform.localEulerAngles.x > camRotatorVerticalAngleMinMax.y && camPivot.transform.localEulerAngles.x < 360 + camRotatorVerticalAngleMinMax.x)
                {
                    if (Mathf.Abs(camPivot.transform.localEulerAngles.x - camRotatorVerticalAngleMinMax.y) < Mathf.Abs(camPivot.transform.localEulerAngles.x - (360 + camRotatorVerticalAngleMinMax.x)))
                    {
                        camPivot.transform.localEulerAngles = new Vector3(camRotatorVerticalAngleMinMax.y, camPivot.transform.localEulerAngles.y, 0);
                    }
                    else
                    {
                        camPivot.transform.localEulerAngles = new Vector3(camRotatorVerticalAngleMinMax.x, camPivot.transform.localEulerAngles.y, 0);
                    }
                }
                // Reset the Z axis of the camera pivot
                camPivot.transform.localEulerAngles = new Vector3(camPivot.transform.localEulerAngles.x, camPivot.transform.localEulerAngles.y, 0);
            }
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    if (groundCheck == null) return;
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(groundCheck.position, groundRange);
    //}

    public IEnumerator LerpToPosition(Vector3 targetPosition, float lerpDuration, System.Action onFinished)
    {
        movingToSpear = true;
        rb.velocity = Vector3.zero;
        Vector3 initialPos = transform.position;

        // Set target position so the target will be at the feets of the player
        targetPosition -= groundCheck.position - transform.position; 

        float timer = 0;

        while (timer < lerpDuration)
        {
            transform.position = Vector3.Lerp(initialPos, targetPosition, timer / lerpDuration);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        movingToSpear = false;

        onFinished?.Invoke();
    }
}
