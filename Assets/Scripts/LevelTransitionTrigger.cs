using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelTransitionTrigger : MonoBehaviour
{
    [Header("Next Level Settings")]
    [Tooltip("Name of the Unity scene to load when the player reaches this point")]
    public string nextSceneName;

    [Header("Completion Audio")]
    public AudioSource audioSource;
    public AudioClip levelCompleteSound;
    public float sceneChangeDelay = 1.5f;

    private bool playerInsideExit = false;
    private bool levelCompleted = false;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Methods for collider entering and exiting the transition area
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInsideExit = true;
            TryAdvanceToNextLevel();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInsideExit = false;
        }
    }

    private void Update()
    {
        if (playerInsideExit && !levelCompleted)
        {
            TryAdvanceToNextLevel();
        }
    }

    private void TryAdvanceToNextLevel()
    {
        if (levelCompleted)
        {
            return;
        }

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning(
                "Next scene name is not set on the trigger!"
            );

            return;
        }

        Enemy[] remainingEnemies =
            FindObjectsByType<Enemy>();

        if (remainingEnemies.Length > 0)
        {
            return;
        }

        levelCompleted = true;

        Debug.Log(
            "All enemies defeated! Loading next level: "
            + nextSceneName
        );

        // Play sound and load next scene
        StartCoroutine(AdvanceToNextLevel());
    }

    private IEnumerator AdvanceToNextLevel()
    {
        if (
            audioSource != null &&
            levelCompleteSound != null
        )
        {
            audioSource.PlayOneShot(levelCompleteSound);
        }

        yield return new WaitForSeconds(
            sceneChangeDelay
        );

        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.LoadSceneWithFade(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}