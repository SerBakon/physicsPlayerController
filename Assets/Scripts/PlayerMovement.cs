using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;

    [Header("Player Inputs")]
    [SerializeField] private KeyCode sprint;
    
    // Private variables
    private Rigidbody rb;

    // Floats
    private float walkingSpeed;
    private float sprintingSpeed;

    // Vector3's
    private Vector3 movement;

    private void Start()
    {
        walkingSpeed = movementSpeed;
        sprintingSpeed = movementSpeed * 1.5f;
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
    }

    private void FixedUpdate()
    {
        moveCharacter(movement);
        Debug.Log(rb.linearVelocity.magnitude);
    }

    private void moveCharacter(Vector3 direction) {
        if(Input.GetKey(sprint)) {
            movementSpeed = sprintingSpeed;
        } else {
            movementSpeed = walkingSpeed;
        }
            rb.linearVelocity = direction * movementSpeed * Time.fixedDeltaTime;
    }

    public float Velocity {
        get { return rb.linearVelocity.magnitude; }
    }
}
