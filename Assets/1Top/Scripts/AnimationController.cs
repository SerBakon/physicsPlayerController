using PurrNet;
using System.Globalization;
using UnityEngine;
public class AnimationController : NetworkBehaviour 
{
    [Header("Gameobjects")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Animation")]
    [SerializeField] private NetworkAnimator animator;

    [Header("Scripts")]
    [SerializeField] private PlayerMovement PlayerMovement;


    private RaycastHit slopeHit;
    protected override void OnSpawned() {
        base.OnSpawned();

        enabled = isOwner;

        //playerPrefab.gameObject.SetActive(!isOwner);
        //Debug.Log("Removing gameobject complete");
    }

    private void Update() {
        if(!onSlope()) {
            switch (PlayerMovement.state) {
                case PlayerMovement.movementState.Sprinting:
                    setSprint();
                    break;
                case PlayerMovement.movementState.Walking:
                    setWalking();
                    break;
                case PlayerMovement.movementState.Sliding:
                    animator.Play("Slide");
                    break;
                case PlayerMovement.movementState.Crouching:
                    setCrouching();
                    break;
                default:
                    animator.Play("Rifle Idle");
                    break;
            }
        } else {
            slopeAnim();
        }
    }

    private void setCrouching() {
        if (Input.GetKey(KeyCode.W)) {
            animator.Play("Crouch Walking");
        } else if (Input.GetKey(KeyCode.S)) {
            animator.Play("Walk Crouching Backward");
        } else if (Input.GetKey(KeyCode.A)) {
            animator.Play("Walk Crouching Left");
        } else if (Input.GetKey(KeyCode.D)) {
            animator.Play("Walk Crouching Right");
        } else {
            animator.Play("Crouch Idle");
        }
    }
    private void setSprint() {
        if (Input.GetKey(KeyCode.W)) {
            animator.Play("Sprint Forward");
        } else if (Input.GetKey(KeyCode.S)) {
            animator.Play("Sprint Backward");
        } else if (Input.GetKey(KeyCode.A)) {
            animator.Play("Sprint Left");
        } else if (Input.GetKey(KeyCode.D)) {
            animator.Play("Sprint Right");
        } else {
            animator.Play("Rifle Idle");
        }
    }
    private void setWalking() {
        if (Input.GetKey(KeyCode.W)) {
            animator.Play("Walking");
        } else if (Input.GetKey(KeyCode.S)) {
            animator.Play("Walking Backwards");
        } else if (Input.GetKey(KeyCode.A)) {
            animator.Play("Walk Left");
        } else if (Input.GetKey(KeyCode.D)) {
            animator.Play("Walk Right");
        } else {
            animator.Play("Rifle Idle");
        }
    }

    private void slopeAnim() {
        switch (PlayerMovement.state) {
            case PlayerMovement.movementState.Sprinting:
                setSprint();
                break;
            case PlayerMovement.movementState.Walking:
                setWalking();
                break;
            case PlayerMovement.movementState.Sliding:
                animator.Play("Slide");
                break;
            default:
                setSprint();
                break;
        }
    }

    private bool onSlope() {

        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, Mathf.Infinity)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            //Debug.Log("On Slope with: " + angle + " Degrees");
            return angle < 60 && angle != 0;
        }

        return false;
    }

}
