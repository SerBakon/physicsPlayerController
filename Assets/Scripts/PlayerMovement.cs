using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCD;

    [SerializeField] private float groundDrag;

    [SerializeField] private float crouchYscale;

    [SerializeField] private float maxSlopeAngle;

    [Header("Player Inputs")]
    [SerializeField] private KeyCode sprint;
    [SerializeField] private KeyCode jump;
    [SerializeField] private KeyCode crouch;

    [Header("Transforms")]
    [SerializeField] private Transform feetPos;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;

    // Private variables
    private Rigidbody rb;

    // Floats
    private float walkingSpeed;
    private float sprintingSpeed;

    private float crouchSpeed;
    private float startYscale;

    // Vector3's
    private Vector3 direction;

    // Bool's
    private bool isGrounded;
    private bool readyToJump;
    private bool exitingSlope;

    // Enums
    private movementState moveState;

    // Raycast
    private RaycastHit slopeHit;
    private enum movementState {
        Idle,
        Walking,
        Sprinting,
        Crouching,
        Air
    }
    private void setValues() {
        readyToJump = true;
        walkingSpeed = movementSpeed;
        sprintingSpeed = movementSpeed * 1.5f;
        crouchSpeed = walkingSpeed * .5f;
        startYscale = transform.localScale.y;
        rb = GetComponent<Rigidbody>();
        if (rb == null) {
            Debug.LogError("No RigidBody Found!");
        }

        moveState = movementState.Idle;
    }
    private void Start()
    {
        setValues();
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
        // Change player scale
        if (moveState == movementState.Crouching) {
            transform.localScale = new Vector3(transform.localScale.x, crouchYscale, transform.localScale.z);
        } else {
            transform.localScale = new Vector3(transform.localScale.x, startYscale, transform.localScale.z);
        }

        if(onSlope() && !exitingSlope) {
            rb.AddForce(GetSlopeDirection() * movementSpeed * 20f, ForceMode.Force);
            if (rb.linearVelocity.y > 0) {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        
        rb.useGravity = !onSlope();

        rb.AddForce(direction * movementSpeed * 10f, ForceMode.Force);
        if (isGrounded) {
            rb.linearDamping = groundDrag;
        } else {
            rb.linearDamping = 0;
        }
        speedControl();
    }

    private void speedControl() {
        if (onSlope() && !exitingSlope) {
            if(rb.linearVelocity.magnitude > movementSpeed) {
                rb.linearVelocity = rb.linearVelocity.normalized * movementSpeed;
            }
        } else {
            Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (horizontalVel.magnitude > movementSpeed) {
                Vector3 limitedVel = horizontalVel.normalized * movementSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private void characterJump() {
        if(isGrounded && Input.GetKey(jump) && readyToJump) {
            readyToJump = false;
            jumpAction();
            Invoke(nameof(resetJump), .5f);
        }
    }
    private void jumpAction() {
        exitingSlope = true;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    private void resetJump() {
        readyToJump = true;
        exitingSlope = false;
    }

    private void setGrounded() {
        if (Physics.CheckSphere(feetPos.position, .15f, groundLayer)) {
            isGrounded = true;
        } else {
            isGrounded = false;
        }
    }

    private void getDirection() {
        direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        direction = transform.TransformDirection(direction);
    }

    private bool onSlope() {
        
        if(Physics.Raycast(feetPos.transform.position, Vector3.down, out slopeHit, .2f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            Debug.Log("On Slope with: " + angle + " Degrees");
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeDirection() {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
    private void setState() {
        // Set Modes
        if (isGrounded && Input.GetKey(sprint)) {
            // Sprinting
            moveState = movementState.Sprinting;
            movementSpeed = sprintingSpeed;
        } else if(isGrounded && Input.GetKey(crouch)) {
            // Crouching
            moveState = movementState.Crouching;
            movementSpeed = crouchSpeed;
        } else if (isGrounded) {
            // Walking
            moveState = movementState.Walking;
            movementSpeed = walkingSpeed;
        } else {
            // In Air
            moveState = movementState.Air;
        }

            //Debug.Log(moveState.ToString());
    }

    // Getters and Setters

    public float Velocity {
        get { return rb.linearVelocity.magnitude; }
    }

    // Debug Gizmos
    private void OnDrawGizmos() {
        // Draws the isGrounded check
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(feetPos.position, 0.15f);
    }
}
