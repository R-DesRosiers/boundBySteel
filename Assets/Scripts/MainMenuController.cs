using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{ // controls buttons for the main menu - play (w/ delay), quit() and back to main menu() for the death screen
    [Header("Button Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public float sceneLoadDelay = 0.2f;

    private bool isLoadingGame = false;

    public void PlayGame()
    { // plays game after little delay so that sound actually plays 
        if (isLoadingGame)
        {
            return;
        }

        isLoadingGame = true;
        StartCoroutine(PlaySoundThenLoadGame());
    }

    private IEnumerator PlaySoundThenLoadGame()
    {
        if (
            audioSource != null &&
            buttonClickSound != null
        )
        {
            audioSource.PlayOneShot(buttonClickSound);
        }

        yield return new WaitForSecondsRealtime(
            sceneLoadDelay
        );

        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.LoadSceneWithFade("Game");
        }
        else
        {
            SceneManager.LoadScene("Game");
        }
    }

    public void QuitGame()
    { // doesnt do anything while testing but will quit game when built? 
        Debug.Log("Quit Game pressed!");
        Application.Quit();
    }

    public void BackToMainMenu()
    { // for death screen and will take the player back to the main menu screen 
        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.LoadSceneWithFade(0);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}