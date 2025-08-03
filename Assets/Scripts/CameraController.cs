using PurrNet;
using System.Globalization;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform camHolder;
    [SerializeField] private Camera playerCam;

    [Header("Sensitivity")]
    [SerializeField] private float sens;

    // Privates

    private float rotX, rotY;
    private float xMin, xMax;

    private Vector3 originalPos;

    // Script Calls

    //private PlayerMovement playerMovement;

    protected override void OnSpawned() {
        base.OnSpawned();

        enabled = isOwner;

        playerCam.gameObject.SetActive(isOwner);
    }

    void Start()
    {
        originalPos = playerCam.transform.localPosition;

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
        playerCam.transform.rotation = Quaternion.Euler(-rotX, rotY, 0);
    }

    public void setCamPos(Vector3 pos) {
        playerCam.transform.localPosition = pos;
    }

    public Vector3 originalCamPos {
        get { return originalPos; }
    }

    public float getRotX {
        get { return rotX; }
    }
}
