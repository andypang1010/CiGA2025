using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int frameRate = 60; // Desired frame rate

    void Start()
    {
        Application.targetFrameRate = frameRate; // Set the target frame rate
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Main Game");
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
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
}
