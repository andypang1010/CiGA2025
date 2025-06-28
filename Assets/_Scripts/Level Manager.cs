using UnityEngine;

public class LevelManager: MonoBehaviour
{

    public float gameTime = 10f;

    public GameObject GameOverUI;
    public GameObject PauseMenuUI;
    public GameObject pauseButton;

    public float remainingTime;

    private void Start()
    {
        remainingTime = gameTime;
        Time.timeScale = 1f; // Ensure the game time starts at normal speed

        GameOverUI.SetActive(false); // Hide the Game Over UI at the start
        PauseMenuUI.SetActive(false); // Hide the Pause Menu UI at the start
        pauseButton.SetActive(true); // Ensure the pause button is active at the start
    }

    private void Update()
    {
        // Check if game is still running
        if (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;

            // If the game is paused
            if (Time.timeScale == 0f)
            {
                GameOverUI.SetActive(false);
                PauseMenuUI.SetActive(true); // Show the Pause Menu UI when the game is paused
                pauseButton.SetActive(false); // Hide the pause button when the game is paused
            }

            // If the game is active
            else
            {
                GameOverUI.SetActive(false);
                PauseMenuUI.SetActive(false); // Hide the Pause Menu UI when the game is active
                pauseButton.SetActive(true);
            }
        }

        else
        {
            // End the game if time runs out
            Time.timeScale = 0f; // Stop the game time

            GameOverUI.SetActive(true);
            PauseMenuUI.SetActive(false); // Show the Pause Menu UI when the game is not active
            pauseButton.SetActive(false); // Hide the pause button when the game is not active
        }
    }

}
