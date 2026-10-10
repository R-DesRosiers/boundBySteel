using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class LevelTransitionTrigger : MonoBehaviour
{
    [Header("Next Level Settings")]
    [Tooltip("Name of the Unity scene to load when the player reaches this point")]
    public string nextSceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object hitting the trigger is the player
        // (Assuming your player GameObject is tagged "Player")
        if (collision.CompareTag("Player"))
        {
            AdvanceToNextLevel();
        }
    }

    void AdvanceToNextLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log("Player reached the end! Loading next level: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name is not set on the trigger!");
        }
    }
}