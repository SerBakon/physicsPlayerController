using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    
    // Private variables
    private Rigidbody rb;

    // Vector3's
    private Vector3 movement;

    private void Start()
    {
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
        rb.linearVelocity = direction * movementSpeed * Time.fixedDeltaTime;
    }

    public float Velocity {
        get { return rb.linearVelocity.magnitude; }
    }
}
