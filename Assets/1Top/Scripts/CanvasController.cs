using PurrNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour {
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI velocityText;
    [SerializeField] private TextMeshProUGUI sensitivityText;

    [Header("Scripts")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CameraController cameraController;

    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup gameView;
    [SerializeField] private CanvasGroup menuView;

    [Header("Binds")]
    [SerializeField] private Slider sensitivitySlider;

    private bool showMenu;

    private void Start() {
        menuView.alpha = 0;
        showMenu = false;
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            toggleView();
        }
        displayVelocity();
        sensitivityText.text = (Mathf.Floor(sensitivitySlider.value * 100f) / 10f).ToString();
    }
    private void toggleView() {
        showMenu = !showMenu;
        if (showMenu) {
            gameView.alpha = 0;
            menuView.alpha = 1;

            playerMovement.GetComponent<PlayerMovement>().enabled = false;
            cameraController.GetComponent<CameraController>().enabled = false;

            setSensitivityBar();

            Cursor.lockState = CursorLockMode.None;

            Debug.Log("Toggling View");
        } else {
            gameView.alpha = 1;
            menuView.alpha = 0;

            setSensitivity();

            playerMovement.GetComponent<PlayerMovement>().enabled = true;
            cameraController.GetComponent<CameraController>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void setSensitivityBar() {
        Debug.Log(cameraController.sensitivity);
        sensitivitySlider.value = cameraController.sensitivity / 10f;
    }

    private void setSensitivity() {
        cameraController.sensitivity = sensitivitySlider.value * 10;
    }

    private void displayVelocity() {
        float velocity = playerMovement.Velocity;
        if (velocity < 0.01) {
            velocity = 0;
        } else {
            velocity = Mathf.Floor(velocity * 10f) / 10f;
        }
        velocityText.text = "Velocity: " + velocity;
    }

    public void setMovementScript() {
        playerMovement = InstanceHandler.GetInstance<PlayerMovement>();
    }
    public void setCameraScript() {
        cameraController = InstanceHandler.GetInstance<CameraController>();
    }
}