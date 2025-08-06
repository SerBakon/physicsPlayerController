using PurrNet;
using System.Collections;
using System.Globalization;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform camHolder;
    [SerializeField] private Camera playerCam;

    [Header("Sensitivity")]
    [SerializeField] private float sens;

    [Header("GameObjects")]
    [SerializeField] private GameObject head;

    [Header("Scripts")]
    [SerializeField] private CanvasController canvasController;
    // Privates

    private float rotX, rotY;
    private float xMin, xMax;

    private float desiredFOV;

    private Vector3 originalPos;

    // Script Calls

    //private PlayerMovement playerMovement;

    protected override void OnSpawned() {
        base.OnSpawned();

        enabled = isOwner;

        canvasController = GameObject.Find("Canvas").GetComponent<CanvasController>();

        InstanceHandler.RegisterInstance(this);
        canvasController.setCameraScript();

        playerCam.gameObject.SetActive(isOwner);
    }

    private void OnDespawned() {
        InstanceHandler.UnregisterInstance<PlayerMovement>();
    }

    void Start()
    {
        originalPos = camHolder.transform.localPosition;

        if (playerCam == null) {
            Debug.LogWarning("No Camera Found!");
        }
        Cursor.lockState = CursorLockMode.Locked;

        rotX = transform.eulerAngles.x;
        rotY = transform.eulerAngles.y;

        xMin = -90f;
        xMax = 90f;
    }

    void Update()
    {
        setRotations();
        turnCamera();


        //Debug.Log(originalPos);
    }

    private void setRotations() {
        rotX += Input.GetAxis("Mouse Y") * sens;
        rotY += Input.GetAxis("Mouse X") * sens;

        rotX = Mathf.Clamp(rotX, xMin, xMax);
    }

    private void turnCamera() {
        transform.rotation = Quaternion.Euler(0, rotY, 0);
        camHolder.transform.rotation = Quaternion.Euler(-rotX, rotY, 0);
    }

    public void setCamPos(Vector3 pos) {
        camHolder.transform.localPosition = pos;
    }
    public void setCamFOV(float fov, float time) {
        desiredFOV = fov;
        StartCoroutine(lerpCamFOV(desiredFOV, time));
    }
    public void setCamTilt(float angle, float time) {
        Debug.Log(playerCam.transform.localEulerAngles);
        StartCoroutine(lerpCamAngle(angle, time));
    }
    private IEnumerator lerpCamAngle(float desiredAngle, float duration) {
        float time = 0;
        float startValue = playerCam.transform.localEulerAngles.z;

        while (time < duration) {
            float newAngle = Mathf.LerpAngle(startValue, desiredAngle, time / duration);
            playerCam.transform.localEulerAngles = new Vector3(
                playerCam.transform.localEulerAngles.x,
                playerCam.transform.localEulerAngles.z,
                newAngle
            );
            time += Time.deltaTime;
            yield return null;
        }

        // Ensure final angle is exact
        playerCam.transform.localEulerAngles = new Vector3(
            playerCam.transform.localEulerAngles.x,
            playerCam.transform.localEulerAngles.y,
            desiredAngle
        );
    }
    private IEnumerator lerpCamFOV(float desiredFOV, float duration) {
        float time = 0;
        float startValue = playerCam.fieldOfView;

        while (time < duration) {
            playerCam.fieldOfView = Mathf.Lerp(startValue, desiredFOV, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        playerCam.fieldOfView = desiredFOV;
    }

    public Vector3 originalCamPos {
        get { return originalPos; }
    }

    public float getRotX {
        get { return rotX; }
    }

    public float fov {
        get { return desiredFOV; }
    }

    public float sensitivity {
        get { return sens; }
        set { sens = value; }
    }
}
