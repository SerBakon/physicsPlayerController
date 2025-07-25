using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;

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

    private void Start()
    {
        walkingSpeed = movementSpeed;
        sprintingSpeed = movementSpeed * 1.5f;
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        getDirection();

        characterJump();

        setGrounded();
    }

    private void FixedUpdate()
    {
        characterMovement(direction);
        //Debug.Log(rb.linearVelocity.magnitude);
    }

    private void characterMovement(Vector3 direction) {
        moveCharacterGround(direction);
    }

    private void moveCharacterGround(Vector3 direction) {
        movementSpeed = Input.GetKey(sprint) ? sprintingSpeed : walkingSpeed;

        Vector3 horizontalVelocity = direction * movementSpeed * Time.fixedDeltaTime;

        horizontalVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = horizontalVelocity;
    }

    private void characterJump() {
        if(isGrounded && Input.GetKeyDown(jump)) {
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

    public float Velocity {
        get { return rb.linearVelocity.magnitude; }
    }
}
