using PurrNet;
using System.Collections;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour {
    // git branch test
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCD;

    [SerializeField] private float groundDrag;

    [SerializeField] private float crouchYscale;

    [SerializeField] private float maxSlopeAngle;

    [Header("Sliding")]
    [SerializeField] private float slideForce;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float minSlideVelocity;
    [SerializeField] private float maxSlideTime;

    [Header("WallRun")]
    [SerializeField] private float wallRunForce;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float wallClimbSpeed;
    [SerializeField] private float wallJumpForce;
    [SerializeField] private float wallJumpSideForce;
    [SerializeField] private float exitWallTime;

    [Header("Player Inputs")]
    [SerializeField] private KeyCode sprint;
    [SerializeField] private KeyCode jump;
    [SerializeField] private KeyCode crouch;

    [Header("Transforms")]
    [SerializeField] private Transform feetPos;

    [SerializeField] private Transform orientation;
    //[SerializeField] private BoxCollider wallCheck;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    // Private variables
    private Rigidbody rb;

    // Floats
    private float walkingSpeed;
    private float sprintingSpeed;
    private float wallrunSpeed;

    private float crouchSpeed;
    private float startYscale;

    private float horizontalInput;
    private float verticalInput;

    private float currentSlideTime;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    private float exitWallTimer;

    // Vector3's
    private Vector3 direction;

    // Bool's
    private bool isGrounded;
    private bool readyToJump;
    private bool exitingSlope;

    private bool sliding;

    private bool wallLeft;
    private bool wallRight;
    private bool exitingWall;

    // Enums
    private movementState moveState;
    private enum movementState {
        Idle,
        Walking,
        Sprinting,
        Crouching,
        Sliding,
        WallRunning,
        Air
    }
    // Raycast
    private RaycastHit slopeHit;

    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;

    // Script References
    private CameraController camController;

    // ---------------------- START / UPDATE FUNCTIONS -------------------------- \\
    protected override void OnSpawned() {
        base.OnSpawned();

        enabled = isOwner;
    }

    private void setValues() {
        readyToJump = true;
        walkingSpeed = movementSpeed;
        sprintingSpeed = movementSpeed * 1.5f;
        crouchSpeed = walkingSpeed * .5f;
        wallrunSpeed = walkingSpeed * 2f;
        camController = GetComponent<CameraController>();
        //startYscale = transform.localScale.y;
        rb = GetComponent<Rigidbody>();
        if (rb == null) {
            Debug.LogError("No RigidBody Found!");
        }
        rb.freezeRotation = true;

        moveState = movementState.Idle;
    }
    private void Start() {
        setValues();
    }
    private void Update() {
        getDirection();

        characterJump();

        setGrounded();

        checkWall();

        // Update slide timer if we're sliding and not on a slope
        if (moveState == movementState.Sliding && !onSlope()) {
            currentSlideTime += Time.deltaTime;
        }

        setState();

        if (Input.GetKeyUp(crouch)) {
            sliding = false;
            currentSlideTime = 0f;

            raiseCam();
        }

        if (Input.GetKey(crouch)) {
            lowerCam();
        }

        // Cannot get smaller hitbox by holding jump and crouch
        if (Input.GetKey(jump) && moveState != movementState.WallRunning) {
            raiseCam();
        }

        if (Input.GetKeyDown(jump) && moveState == movementState.WallRunning) {
            wallJump();
            Debug.Log("attempting wall jump");
        }
        if (exitingWall) {
            if(exitWallTimer > 0) {
                exitWallTimer -= Time.deltaTime;
            }
            if (exitWallTimer <= 0) {
                exitingWall = false;
            }
        }

        if ((wallRight || wallLeft) && camController.fov != 90 && !exitingWall) {
            //Debug.Log("FOV to 90");
            if (wallLeft) camController.setCamTilt(-5f, .2f);
            if (wallRight) camController.setCamTilt(5f, .2f);
            camController.setCamFOV(90f, 0.2f);
        }
        
        if (!wallRight && !wallLeft && camController.fov == 90) {
            //Debug.Log("FOV to 60");
            camController.StopAllCoroutines();
            camController.setCamFOV(60f, 0.2f);
            camController.setCamTilt(0f, .2f);
        }
        //Debug.Log((wallRight || wallLeft) + " " + (!wallRight && !wallLeft));

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        //Debug.Log(rb.linearVelocity.y);
    }

    private void FixedUpdate() {
        moveCharacterGround(direction);
        
        //Debug.Log(rb.linearVelocity.magnitude);
    }

    // ---------------------- MOVEMENT -------------------------- \\

    private void moveCharacterGround(Vector3 direction) {
        if (rb.linearVelocity.y > 0 && onSlope() && moveState == movementState.Sliding) {
            moveState = movementState.Crouching;
            Debug.Log("Do not slide up slopes");
        }
        
        switch (moveState) {
            case movementState.Air:
                airControl();
                break;
            case movementState.Sliding:
                slidingMovement();
                break;
            case movementState.WallRunning:
                wallrunMovement();
                break;
            default:
                toggleWalk();
                break;

        }
        //Debug.Log(moveState.ToString());

        // On Slope
        if (onSlope() && !exitingSlope) {
            rb.AddForce(GetSlopeDirection() * movementSpeed * 20f, ForceMode.Force);
            if (rb.linearVelocity.y > 0) {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        rb.useGravity = !onSlope();

        speedControl();

        //Debug.Log(rb.linearVelocity.y);
    }

    private void speedControl() {
        if (onSlope() && !exitingSlope) {
            if(rb.linearVelocity.magnitude > movementSpeed) {
                rb.linearVelocity = rb.linearVelocity.normalized * movementSpeed;
            }
        } else {
            Vector3 horizontalVel = getHorizontalVelocity();

            if (horizontalVel.magnitude > movementSpeed) {
                Vector3 limitedVel = horizontalVel.normalized * movementSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private void airControl() {
        rb.AddForce(direction * movementSpeed * 10f, ForceMode.Force);
        // Multiply linearVel.y for better gravity feel
        if(rb.linearVelocity.y < 0) {
            rb.AddForce(new Vector3(0, rb.linearVelocity.y * 5.0f, 0));
        }
    }
    // ---------------------- JUMPING -------------------------- \\

    private void characterJump() {
        if(isGrounded && Input.GetKey(jump) && readyToJump) {
            readyToJump = false;
            jumpAction();
            Invoke(nameof(resetJump), .5f);
        }
    }
    private void jumpAction() {
        exitingSlope = true;
        //Debug.Log(exitingSlope);
        rb.linearVelocity = getHorizontalVelocity();
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    private void resetJump() {
        readyToJump = true;
        exitingSlope = false;
    }
    // ---------------------- SLIDING -------------------------- \\
    private void slidingMovement() {

        if(!onSlope() || rb.linearVelocity.y > -0.1f) {
            desiredMoveSpeed = sprintingSpeed;
            rb.AddForce(direction.normalized * slideForce, ForceMode.Force);
        } else {
            desiredMoveSpeed = slideSpeed;
            // Cannot Slide up slopes
            rb.AddForce(GetSlopeDirection() * slideForce, ForceMode.Force);
        }
    }
    // ---------------------- CROUCHING -------------------------- \\
    //private void startCrouch() {
    //    toggleWalk();
    //    Debug.Log("Crouching");
    //}

    private void lowerCam() {
        camController.setCamPos(camController.originalCamPos + new Vector3(0, -0.5f, 0));
    }
    private void raiseCam() {
        camController.setCamPos(camController.originalCamPos);

    }
    // ---------------------- WALLRUNNING -------------------------- \\
    private void checkWall() {
        wallRight = Physics.Raycast(orientation.transform.position, orientation.right, out rightWallHit, wallCheckDistance, wallLayer);
        wallLeft = Physics.Raycast(orientation.transform.position, -orientation.right, out leftWallHit, wallCheckDistance, wallLayer);
    }
    private void wallrunMovement() {
        if (!exitingWall) {
            rb.useGravity = false;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            //camController.setCamFOV(90f, 0.3f);

            Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

            Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

            if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude) {
                wallForward = -wallForward;
            }

            rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

            //Scale up the wall by how far up the camera faces and fixes falling problem
            float scalingPercent = camController.getRotX / 90f;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, (wallClimbSpeed * scalingPercent) + .2f, rb.linearVelocity.z);


            if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0)) {
                rb.AddForce(-wallNormal * 100, ForceMode.Force);
            }
        }
    }
    private void wallJump() {
        exitingWall = true;
        exitWallTimer = exitWallTime;
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallJumpApply = (transform.up * wallJumpForce) + (wallNormal * wallJumpSideForce);

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(wallJumpApply, ForceMode.Impulse);
    }

    // ---------------------- WALKING / SPRINT -------------------------- \\
    private void toggleWalk() {
        // Normal Running/Sprinting
        rb.AddForce(direction * movementSpeed * 10f, ForceMode.Force);
        if (isGrounded) {
            rb.linearDamping = groundDrag;
        } else {
            rb.linearDamping = 0;
        }
    }

    // ---------------------- MOVEMENT HELPER FUNCTIONS -------------------------- \\
    private IEnumerator smoothlyLerpSpeed() {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - movementSpeed);
        float startVal = movementSpeed;

        while (time < difference) {
            movementSpeed = Mathf.Lerp(startVal, desiredMoveSpeed, (time / difference) * 5f);
            time += Time.deltaTime;
            yield return null;
        }
        movementSpeed = desiredMoveSpeed;
    }

    private void setGrounded() {
        if (Physics.CheckSphere(feetPos.position, .15f, groundLayer)) {
            isGrounded = true;
        } else {
            isGrounded = false;
        }
        //Debug.Log(isGrounded);
    }

    private void getDirection() {
        direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        direction = transform.TransformDirection(direction);
    }

    private bool onSlope() {
        
        if(Physics.Raycast(feetPos.transform.position, Vector3.down, out slopeHit, .2f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            //Debug.Log("On Slope with: " + angle + " Degrees");
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeDirection() {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private Vector3 getHorizontalVelocity() {
        return new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    }

    // ---------------------- STATE HANDLING -------------------------- \\

    private void setState() {
        // Set Modes
        if((wallLeft || wallRight) && verticalInput > 0 && !isGrounded) {
            // Wall Running
            moveState = movementState.WallRunning;
            desiredMoveSpeed = wallrunSpeed;
        } else if (isGrounded && Input.GetKey(sprint) && !Input.GetKey(crouch)) {
            // Sprinting
            moveState = movementState.Sprinting;
            desiredMoveSpeed = sprintingSpeed;
        } else if (isGrounded && Input.GetKey(crouch)) {
            if (rb.linearVelocity.magnitude > minSlideVelocity) {
                // Start sliding only if we weren't already sliding
                if (!sliding) {
                    sliding = true;
                    currentSlideTime = 0f;
                }

                // Continue sliding if we have time left or are on a slope
                if (currentSlideTime < maxSlideTime) {
                    moveState = movementState.Sliding;
                } else {
                    // Time expired - force crouch
                    moveState = movementState.Crouching;
                    desiredMoveSpeed = crouchSpeed;
                }
            } else {
                // Crouching
                moveState = movementState.Crouching;
                desiredMoveSpeed = crouchSpeed;
                sliding = false;
            }
        } else if (isGrounded) {
            // Walking
            moveState = movementState.Walking;
            desiredMoveSpeed = walkingSpeed;
        } else {
            // In Air
            moveState = movementState.Air;
        }
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && movementSpeed != 0) {
            StopAllCoroutines();
            StartCoroutine(smoothlyLerpSpeed());
        } else {
            movementSpeed = desiredMoveSpeed;
        }
        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    // ---------------------- GETTERS AND SETTERS -------------------------- \\

    public float Velocity {
        get { return rb.linearVelocity.magnitude; }
    }

    // ---------------------- DEBUG GIZMOS -------------------------- \\
    private void OnDrawGizmos() {
        // Draws the isGrounded check
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(feetPos.position, 0.15f);
    }
}
