using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game pressed!");
        Application.Quit();
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}