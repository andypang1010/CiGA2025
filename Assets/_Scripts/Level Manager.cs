using UnityEngine;

public class LevelManager: MonoBehaviour
{
    /// <summary>
    /// THE DURATION OF ONE GAME
    /// </summary>
    public float GAMETIME = 180f;
    private bool gameIsActive = true;

    private void Update()
    {
        if (gameIsActive)
        {
            GAMETIME -= Time.deltaTime;
            if (GAMETIME <= 0.01f)
            {
                gameIsActive = false;
            }
        }
    }

}
