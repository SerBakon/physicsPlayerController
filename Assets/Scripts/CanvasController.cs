using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI velocityText;

    [Header("Scripts")]
    [SerializeField] private PlayerMovement playerMovement;

    private void Update() {
        displayVelocity();
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
}
