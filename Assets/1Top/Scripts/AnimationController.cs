using PurrNet;
using System.Globalization;
using UnityEngine;

public class AnimationController : NetworkBehaviour 
{
    [Header("Gameobjects")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject gameObjectPrefab;

    [Header("Animation")]
    [SerializeField] private NetworkAnimator animator;
    [SerializeField] private AnimationClip slopeSlideAnim;

    [Header("Scripts")]
    [SerializeField] private PlayerMovement PlayerMovement;


    private RaycastHit slopeHit;
    private string currentAnim;

    private bool startedSlopeSlide;
    protected override void OnSpawned() {
        base.OnSpawned();

        enabled = isOwner;

        //playerPrefab.gameObject.SetActive(!isOwner);
        Debug.Log("Removing gameobject complete");        
    }

    private void Update() {
        if (!onSlope() && isOwner) {
            gameObjectPrefab.transform.localEulerAngles = Vector3.zero;
            switch (PlayerMovement.state) {
                case PlayerMovement.movementState.Sprinting:
                    setSprint();
                    break;
                case PlayerMovement.movementState.Walking:
                    setWalking();
                    break;
                case PlayerMovement.movementState.Sliding:
                    slideGround();
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
        if (Input.GetKeyUp(PlayerMovement.crouchKey)) {
            startedSlopeSlide = false;
        }
    }
    private void slideGround() {
        if(currentAnim != "Continuous Slide")
            changeAnimation("Slide");
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
                slopeSlide();
                break;
            case PlayerMovement.movementState.Crouching:
                changeAnimation("Crouch Idle");
                break;
            case PlayerMovement.movementState.Air:
                slopeAir();
                break;
            default:
                setSprint();
                break;
        }
        if (PlayerMovement.state == PlayerMovement.movementState.Sliding) {
            gameObjectPrefab.transform.localEulerAngles = new Vector3(PlayerMovement.slopeAnglePub, 0, 0);
        } else {
            gameObjectPrefab.transform.localEulerAngles = Vector3.zero;
        }
    }

    private void slopeAir() {
        if (Input.GetKey(PlayerMovement.sprintKey)) {
            setSprint();
        } else {
            setWalking();
        }
    }
    private void slopeSlide() {
        if(!startedSlopeSlide) {
            changeAnimation("Slope Slide");
            startedSlopeSlide = true;
        }
        
        Invoke("continuousSlide", slopeSlideAnim.length);
    }
    private void continuousSlide() {
        changeAnimation("Continuous Slide");
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
