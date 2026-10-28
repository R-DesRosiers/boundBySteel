using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.35f;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeCanvasGroup == null)
        {
            fadeCanvasGroup =
                GetComponentInChildren<CanvasGroup>();
        }

        fadeCanvasGroup.alpha = 1f;

        SceneManager.sceneLoaded += OnSceneLoaded;

        StartCoroutine(
            FadeCanvas(1f, 0f)
        );
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        isTransitioning = false;

        StartCoroutine(
            FadeCanvas(1f, 0f)
        );
    }

    public void LoadSceneWithFade(string sceneName)
    {
        if (isTransitioning)
        {
            return;
        }

        isTransitioning = true;

        StartCoroutine(
            FadeAndLoadScene(sceneName)
        );
    }

    public void LoadSceneWithFade(int buildIndex)
    {
        if (isTransitioning)
        {
            return;
        }

        isTransitioning = true;

        StartCoroutine(
            FadeAndLoadScene(buildIndex)
        );
    }

    private IEnumerator FadeAndLoadScene(
        string sceneName
    )
    {
        yield return FadeCanvas(0f, 1f);

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeAndLoadScene(
        int buildIndex
    )
    {
        yield return FadeCanvas(0f, 1f);

        SceneManager.LoadScene(buildIndex);
    }

    private IEnumerator FadeCanvas(float startingAlpha, float endingAlpha)
    {
        float elapsed = 0f;

        fadeCanvasGroup.alpha = startingAlpha;

        // Allow the fade panel to block clicks while fading
        fadeCanvasGroup.blocksRaycasts = true;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            fadeCanvasGroup.alpha = Mathf.Lerp(
                startingAlpha,
                endingAlpha,
                elapsed / fadeDuration
            );

            yield return null;
        }

        fadeCanvasGroup.alpha = endingAlpha;

        // Stop blocking UI clicks once the fade is invisible
        fadeCanvasGroup.blocksRaycasts = endingAlpha > 0.01f;
    }
}