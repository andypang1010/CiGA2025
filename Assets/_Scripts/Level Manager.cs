using UnityEngine;

public class LevelManager: MonoBehaviour
{

    public const float gameTime = 10f;
    public GameObject GameOverUI;

    float remainingTime;
    bool gameIsActive = true;

    private void Start()
    {
        remainingTime = gameTime;
        Time.timeScale = 1f; // Ensure the game time starts at normal speed
        GameOverUI.SetActive(false); // Hide the Game Over UI at the start
    }

    private void Update()
    {
        if (gameIsActive)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0f)
            {
                gameIsActive = false;
                Time.timeScale = 0f; // Stop the game time
                GameOverUI.SetActive(true);
            }
        }

    }

}
