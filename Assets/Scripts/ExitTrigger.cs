using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ExitTrigger : MonoBehaviour
{
    [Header("Final Level Settings")]
    [SerializeField] private string nextSceneName = "ExitScreen";

    [Header("Completion Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip levelCompleteSound;
    [SerializeField] private float sceneChangeDelay = 1.5f;

    private bool playerInsideExit = false;
    private bool levelCompleted = false;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideExit = true;
            TryCompleteLevel();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideExit = false;
        }
    }

    private void Update()
    {
        // Level ends only when all enemies are gone
        // and the player is inside the exit.
        if (playerInsideExit && !levelCompleted)
        {
            TryCompleteLevel();
        }
    }

    private void TryCompleteLevel()
    {
        if (levelCompleted)
        {
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
            "All enemies defeated! Completing final level."
        );

        StartCoroutine(CompleteLevel());
    }

    private IEnumerator CompleteLevel()
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

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            if (SceneFader.Instance != null)
            {
                SceneFader.Instance.LoadSceneWithFade(nextSceneName);
            }
            else
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
        else
        {
            Debug.LogWarning(
                "Next scene name is not assigned."
            );
        }
    }
}