using PurrNet;
using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour {
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI velocityText;

    [Header("Scripts")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup gameView;
    [SerializeField] private CanvasGroup menuView;

    private bool showMenu = false;

    private void Start() {
        menuView.alpha = 0;
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.M)) {
            toggleView();
        }
        displayVelocity();
    }
    private void toggleView() {
        if (showMenu) {
            gameView.alpha = 0;
            menuView.alpha = 1;
        } else {
            gameView.alpha = 1;
            menuView.alpha = 0;
        }
        showMenu = !showMenu;
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
}