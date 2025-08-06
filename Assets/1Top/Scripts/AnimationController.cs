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
    private string currentAnim;
    protected override void OnSpawned() {
        base.OnSpawned();

        enabled = isOwner;

        playerPrefab.gameObject.SetActive(!isOwner);
        Debug.Log("Removing gameobject complete");        
    }

    private void Update() {
        if (!onSlope() && isOwner) {
            switch (PlayerMovement.state) {
                case PlayerMovement.movementState.Sprinting:
                    setSprint();
                    break;
                case PlayerMovement.movementState.Walking:
                    setWalking();
                    break;
                case PlayerMovement.movementState.Sliding:
                    changeAnimation("Slide");
                    break;
                case PlayerMovement.movementState.Crouching:
                    setCrouching();
                    break;
                case PlayerMovement.movementState.WallRunning:
                    setSprint();
                    break;
                case PlayerMovement.movementState.Air:
                    changeAnimation("InAir");
                    break;
                case PlayerMovement.movementState.Climbing:
                    changeAnimation("Climbing");
                    break;
                default:
                    changeAnimation("Rifle Idle");
                    break;
            }
        } else if(onSlope() && isOwner) {
            slopeAnim();
        }
    }

    private void setCrouching() {
        if (Input.GetKey(KeyCode.W)) {
            changeAnimation("Crouch Walking");
            //changeAnimation("Crouch Walking");
        } else if (Input.GetKey(KeyCode.S)) {
            changeAnimation("Walk Crouching Backward");
            //changeAnimation("Walk Crouching Backward");
        } else if (Input.GetKey(KeyCode.A)) {
            changeAnimation("Walk Crouching Left");
            //changeAnimation("Walk Crouching Left");
        } else if (Input.GetKey(KeyCode.D)) {
            changeAnimation("Walk Crouching Right");
            //changeAnimation("Walk Crouching Right");
        } else {
            changeAnimation("Crouch Idle");
            //changeAnimation("Crouch Idle");
        }
    }
    private void setSprint() {
        if (Input.GetKey(KeyCode.W)) {
            changeAnimation("Sprint Forward");
            //changeAnimation("Sprint Forward");
        } else if (Input.GetKey(KeyCode.S)) {
            changeAnimation("Sprint Backward");
            //changeAnimation("Sprint Backward");
        } else if (Input.GetKey(KeyCode.A)) {
            changeAnimation("Sprint Left");
            //changeAnimation("Sprint Left");
        } else if (Input.GetKey(KeyCode.D)) {
            changeAnimation("Sprint Right");
            //changeAnimation("Sprint Right");
        } else {
            changeAnimation("Rifle Idle");
            //changeAnimation("Rifle Idle");
        }
    }
    private void setWalking() {
        if (Input.GetKey(KeyCode.W)) {
            changeAnimation("Walking");
        } else if (Input.GetKey(KeyCode.S)) {
            changeAnimation("Walking Backwards");
        } else if (Input.GetKey(KeyCode.A)) {
            changeAnimation("Walk Left");
        } else if (Input.GetKey(KeyCode.D)) {
            changeAnimation("Walk Right");
        } else {
            changeAnimation("Rifle Idle");
        }
    }

    private void slopeAnim() {
        if(PlayerMovement.VelocityY > 0) {
            setSprint();
            return;
        }
        switch (PlayerMovement.state) {
            case PlayerMovement.movementState.Sprinting:
                setSprint();
                break;
            case PlayerMovement.movementState.Walking:
                setWalking();
                break;
            case PlayerMovement.movementState.Sliding:
                changeAnimation("Slide");
                break;
            case PlayerMovement.movementState.Crouching:
                changeAnimation("Crouch Idle");
                break;
            default:
                setSprint();
                break;
        }
    }
    private void changeAnimation(string animation, float crossfade = 0.05f) {
        if (currentAnim != animation) {
            currentAnim = animation;
            animator.CrossFade(animation, crossfade);
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
