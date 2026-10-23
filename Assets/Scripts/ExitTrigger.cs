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
        if (
            other.CompareTag("Player") &&
            !levelCompleted
        )
        {
            levelCompleted = true;
            StartCoroutine(CompleteLevel());
        }
    }

    private IEnumerator CompleteLevel()
    {
        if (audioSource != null && levelCompleteSound != null)
        {
            audioSource.PlayOneShot(levelCompleteSound);
        }

        yield return new WaitForSeconds(sceneChangeDelay);

        SceneManager.LoadScene(nextSceneName);
    }
}