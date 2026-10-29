using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pausePanel;

    [Header("Gameplay Text")]
    public TMP_Text enemiesSpawningText;

    [Header("Background Audio")]
    public AudioSource backgroundAudio;

    [Header("Mute Button Text")]
    public TMP_Text muteButtonText;

    private bool isPaused = false;
    private bool isMuted = false;

    private void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (enemiesSpawningText != null)
        {
            enemiesSpawningText.enabled = true;
        }

        Time.timeScale = 1f;

        if (backgroundAudio != null)
        {
            backgroundAudio.mute = false;
        }

        UpdateMuteButtonText();
    }

    private void Update()
    {
        // Listen for the Escape key
        if (
            Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame
        )
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        // Pause gameplay and show the pause menu
        isPaused = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        if (enemiesSpawningText != null)
        {
            enemiesSpawningText.enabled = false;
        }

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        // Hide the pause menu and resume gameplay
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (enemiesSpawningText != null)
        {
            enemiesSpawningText.enabled = true;
        }

        Time.timeScale = 1f;
    }

    public void ToggleMute()
    {
        // Mute or unmute the background forest audio
        isMuted = !isMuted;

        if (backgroundAudio != null)
        {
            backgroundAudio.mute = isMuted;
        }

        UpdateMuteButtonText();
    }

    private void UpdateMuteButtonText()
    {
        if (muteButtonText != null)
        {
            muteButtonText.text =
                isMuted
                ? "Unmute Background Noise"
                : "Mute Background Noise";
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}