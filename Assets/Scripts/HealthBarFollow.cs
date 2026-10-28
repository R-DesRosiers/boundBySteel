using UnityEngine;
using UnityEngine.UI; 

public class HealthBarFollow : MonoBehaviour
{
    public Transform playerTransform; // Drag your Player here in the Inspector
    public PlayerController playerController; 
    public UnityEngine.UI.Image healthBarFill; 
    public Vector3 offset = new Vector3(0f, .1f, 0f); // Height above the player's head

    void LateUpdate()
    {
        // 1. Follow the player's head
        if (playerTransform != null)
        {
            transform.position = playerTransform.position + offset;
        }

        // 2. Update the fill amount dynamically based on PlayerController health
        if (playerController != null && healthBarFill != null)
        {
            float healthPercent = (float)playerController.health / playerController.maxHealth;
            healthBarFill.fillAmount = healthPercent;
        }
    }
}