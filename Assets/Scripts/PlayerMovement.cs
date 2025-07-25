using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;

    [SerializeField] private float groundDrag;

    [Header("Player Inputs")]
    [SerializeField] private KeyCode sprint;
    [SerializeField] private KeyCode jump;

    [Header("Transforms")]
    [SerializeField] private Transform feetPos;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;

    // Private variables
    private Rigidbody rb;

    // Floats
    private float walkingSpeed;
    private float sprintingSpeed;

    // Vector3's
    private Vector3 direction;

    // Bool's
    private bool isGrounded;

    // Enums
    private movementState moveState;
    private enum movementState {
        Idle,
        Walking,
        Sprinting,
        Air
    }

    private void Start()
    {
        walkingSpeed = movementSpeed;
        sprintingSpeed = movementSpeed * 1.5f;
        rb = GetComponent<Rigidbody>();

        moveState = movementState.Idle;
    }

    private void Update() {
        getDirection();

        characterJump();

        setGrounded();

        setState();
    }

    private void FixedUpdate()
    {
        characterMovement(direction);
        //Debug.Log(rb.linearVelocity.magnitude);
    }

    private void characterMovement(Vector3 direction) {
        moveCharacterGround(direction);
    }

    // Ground Movement
    private void moveCharacterGround(Vector3 direction) {
        //movementSpeed = Input.GetKey(sprint) ? sprintingSpeed : walkingSpeed;

        rb.AddForce(direction * movementSpeed * 10f, ForceMode.Force);

        if (isGrounded) {
            rb.linearDamping = groundDrag;
        } else {
            rb.linearDamping = 0;
        }
        speedControl();
    }

    private void speedControl() {
        Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if(horizontalVel.magnitude > movementSpeed) {
            Vector3 limitedVel = horizontalVel.normalized * movementSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void characterJump() {
        if(isGrounded && Input.GetKey(jump)) {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void setGrounded() {
        if (Physics.CheckSphere(feetPos.position, .5f, groundLayer)) {
            isGrounded = true;
        } else {
            isGrounded = false;
        }
    }

    private void getDirection() {
        direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        direction = transform.TransformDirection(direction);
    }

    private void setState() {
        // Set Modes
        if (isGrounded && Input.GetKey(sprint)) {
            // Sprinting
            moveState = movementState.Sprinting;
            movementSpeed = sprintingSpeed;
        } else if (isGrounded) {
            // Walking
            moveState = movementState.Walking;
            movementSpeed = walkingSpeed;
        } else {
            // In Air
            moveState = movementState.Air;
        }

            Debug.Log(moveState.ToString());
    }

    // Getters and Setters

    public float Velocity {
        get { return rb.linearVelocity.magnitude; }
    }
}
