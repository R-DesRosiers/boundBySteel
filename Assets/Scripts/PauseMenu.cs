using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pausePanel;

    [Header("Background Audio")]
    public AudioSource backgroundAudio;

    [Header("Mute Button Text")]
    public TMP_Text muteButtonText;

    private bool isPaused = false;
    private bool isMuted = false;

    private void Start()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        if (backgroundAudio != null)
        {
            backgroundAudio.mute = false;
        }

        UpdateMuteButtonText();
    }

    private void Update()
    { // listens for escape key to be pressed by player 
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
    { // STOPS / FREEZES gameplay and displays pause panel of buttons / darkened background 
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    { // hides pause panel and resumes gameplay 
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ToggleMute()
    { // have forest noise as background, per requirements that can be muted 
        isMuted = !isMuted;

        if (backgroundAudio != null)
        {
            backgroundAudio.mute = isMuted;
        }

        UpdateMuteButtonText();
    }

    private void UpdateMuteButtonText()
    { // if clicked --> switch text for button 
        if (muteButtonText != null)
        {
            muteButtonText.text =
                isMuted ? "Unmute Music" : "Mute Music";
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void BackToMainMenu()
    { // go back to zeroth / first scene (which is main menu in this case)
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}