using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    Rigidbody rb;

    [Header("Speeds")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeed;

    [Space(10)] [Header("Cam")]
    [SerializeField] private Transform camRotator;
    [SerializeField] private Vector2 camRotatorAngleMinMax;

    //private bool isGrounded;
    //[Space(10)]
    //[Header("Ground Check")]
    //[SerializeField] private LayerMask groundMask;
    //[SerializeField] private float groundRange;
    //[SerializeField] private Transform groundCheck;
    //[SerializeField] private float jumpForce;

    [Space(10)]
    [SerializeField] private float horizontalMouseSensivity;
    [SerializeField] private float verticalMouseSensivity;

    bool movingToSpear;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!movingToSpear)
        {
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            //bool isGrounded = Physics.CheckSphere(groundCheck.position, groundRange, groundMask);

            //if (isGrounded)
            //{
            if (input == Vector2.zero) rb.velocity = new Vector3(0, rb.velocity.y, 0);
            else
            {
                float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;

                input.Normalize();
                Vector3 moveDirection = transform.forward * input.y + input.x * transform.right;
                //Debug.Log(moveDirection);

                Vector3 movement = moveDirection * currentSpeed;
                movement.y = rb.velocity.y;

                rb.velocity = movement;
            }

                /// No more jump ;(

                //if (Input.GetKeyDown(KeyCode.Space))
                //{
                //    rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
                //}
            //}

            //Debug.DrawRay(transform.position, transform.forward, Color.green);
            //Debug.DrawRay(transform.position, transform.right, Color.red);

            // Rotate the player on the Y axis depending on the MouseX axis
            float rotX = Input.GetAxis("Mouse X") * horizontalMouseSensivity * Mathf.Deg2Rad;
            transform.Rotate(Vector3.up, rotX);

            // Rotate the cam on the X axis depending on the MouseY axis
            float rotY = Input.GetAxis("Mouse Y") * verticalMouseSensivity * Mathf.Deg2Rad;
            camRotator.Rotate(Vector3.right, -rotY);

            // Lock Camera between two max angle on the X axis
            if(camRotator.localEulerAngles.x > camRotatorAngleMinMax.y && camRotator.localEulerAngles.x < 360 + camRotatorAngleMinMax.x)
            {
                if (Mathf.Abs(camRotator.localEulerAngles.x - camRotatorAngleMinMax.y) < Mathf.Abs(camRotator.localEulerAngles.x - (360 + camRotatorAngleMinMax.x)))
                {
                    camRotator.localEulerAngles =  new Vector3(camRotatorAngleMinMax.y, 0, 0);
                }
                else
                {
                    camRotator.localEulerAngles =  new Vector3(camRotatorAngleMinMax.x, 0, 0);
                }
            }
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    if (groundCheck == null) return;
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(groundCheck.position, groundRange);
    //}

    public IEnumerator LerpToPosition(Vector3 targetPosition, float lerpDuration)
    {
        movingToSpear = true;
        rb.velocity = Vector3.zero;
        Vector3 initialPos = transform.position;

        float timer = 0;

        while(timer < lerpDuration)
        {
            transform.position = Vector3.Lerp(initialPos, targetPosition, timer / lerpDuration);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        movingToSpear = true;
    }
}
