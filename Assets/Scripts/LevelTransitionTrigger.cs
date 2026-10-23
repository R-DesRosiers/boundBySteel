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

    private bool levelCompleted = false;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (
            collision.CompareTag("Player") &&
            !levelCompleted
        )
        {
            levelCompleted = true;
            StartCoroutine(AdvanceToNextLevel());
        }
    }

    private IEnumerator AdvanceToNextLevel()
    {
        if (audioSource != null && levelCompleteSound != null)
        {
            audioSource.PlayOneShot(levelCompleteSound);
        }

        yield return new WaitForSeconds(sceneChangeDelay);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log(
                "Player reached the end! Loading next level: "
                + nextSceneName
            );

            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning(
                "Next scene name is not set on the trigger!"
            );
        }
    }
}