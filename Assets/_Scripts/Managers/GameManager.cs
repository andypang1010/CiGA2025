using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int frameRate = 60; // Desired frame rate

    [Tooltip("Assign your full-screen black Image with a CanvasGroup here")]
    public CanvasGroup screenFader;

    [Tooltip("Seconds to fade in/out")]
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        screenFader.alpha = 0f;
    }

    void Start()
    {
        Application.targetFrameRate = frameRate; // Set the target frame rate
    }

    public void StartGame()
    {
        FadeAndLoad("MAIN GAME");
    }

    public void Menu()
    {
        FadeAndLoad("Menu");
    }

    public void Tutorial()
    {
        FadeAndLoad("Tutorial");
    }

    public void PauseGame()
    {
        Time.timeScale = 0; // Pause the game by setting time scale to 0
    }

    public void ResumeGame()
    {
        Time.timeScale = 1; // Resume the game by setting time scale back to 1
    }

    public void EndGame()
    {
        Application.Quit(); // Quit the application
    }

    /// <summary>
    /// Fade to black, then load the scene, then fade back in.
    /// </summary>
    public void FadeAndLoad(string sceneName)
    {
        StartCoroutine(FadeCoroutine(sceneName));
    }

    private IEnumerator FadeCoroutine(string sceneName)
    {
        // Fade to black
        yield return StartCoroutine(ChangeAlpha(0f, 1f));

        // Load the new scene (synchronous)
        SceneManager.LoadScene(sceneName);

        // (Optional) small wait for first frame to render
        yield return null;

        // Fade back in
        yield return StartCoroutine(ChangeAlpha(1f, 0f));
    }

    private IEnumerator ChangeAlpha(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            screenFader.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        screenFader.alpha = to;
    }
}
