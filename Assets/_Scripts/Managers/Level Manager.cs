using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LevelManager: MonoBehaviour
{

    public float gameTime = 10f;
    public float remainingTime;

    public GameObject GoodEndingUI;
    public GameObject SoSoEndingUI;
    public GameObject BadEndingUI;
    public GameObject statsUI;
    public GameObject PauseMenuUI;
    public GameObject pauseButton;
    public GameObject timer;

    public Volume volume;
    Vignette vignette;


    private void Start()
    {
        remainingTime = gameTime;
        Time.timeScale = 1f; // Ensure the game time starts at normal speed

        GoodEndingUI.SetActive(false); // Hide the Good Ending UI at the start
        SoSoEndingUI.SetActive(false); // Hide the So-So Ending UI at the start
        BadEndingUI.SetActive(false); // Hide the Game Over UI at the start
        statsUI.SetActive(false); // Hide the stats UI at the start
        PauseMenuUI.SetActive(false); // Hide the Pause Menu UI at the start
        pauseButton.SetActive(true); // Ensure the pause button is active at the start

        if (volume != null)
        {
            volume.enabled = true; // Enable the volume component if it exists
            volume.profile.TryGet(out vignette); // Try to get the Vignette effect from the volume profile
        }
        else
        {
            Debug.LogWarning("Volume component is not assigned in LevelManager.");
        }
    }

    private void Update()
    {
        timer.GetComponent<TMP_Text>().text = "Time Left: " + remainingTime.ToString("f1");

        // Check if game is still running
        if (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            vignette.intensity.value = 0.4f;

            // If the game is paused
            if (Time.timeScale == 0f)
            {
                GoodEndingUI.SetActive(false);
                SoSoEndingUI.SetActive(false);
                BadEndingUI.SetActive(false);
                statsUI.SetActive(false); // Show the stats UI when the game is paused
                PauseMenuUI.SetActive(true); // Show the Pause Menu UI when the game is paused
                pauseButton.SetActive(false); // Hide the pause button when the game is paused
                timer.SetActive(false);
            }

            // If the game is active
            else
            {
                GoodEndingUI.SetActive(false);
                SoSoEndingUI.SetActive(false);
                BadEndingUI.SetActive(false);
                PauseMenuUI.SetActive(false); // Hide the Pause Menu UI when the game is active
                pauseButton.SetActive(true);
                timer.SetActive(true);
            }
        }

        else
        {
            // End the game if time runs out
            Time.timeScale = 0f; // Stop the game time

            List<GameObject> plantsList = GameObject.FindGameObjectsWithTag("Plant").ToList();
            int numPlantsAlive = plantsList.Count(plant => !plant.GetComponent<Plant>().isDead);

            List<GameObject> cowsList = GameObject.FindGameObjectsWithTag("Cow").ToList();
            int numCowsAlive = cowsList.Count(cow => !cow.GetComponent<Cow>().isDead);

            // Determine the ending based on the number of alive plants and cows
            if (numPlantsAlive == plantsList.Count && numCowsAlive == cowsList.Count)
            {
                // If all plants and cows are alive, show Good Ending UI
                GoodEndingUI.SetActive(true);
                SoSoEndingUI.SetActive(false);
                BadEndingUI.SetActive(false);
          
            }
            else if (numPlantsAlive > 0 || numCowsAlive > 0)
            {
                // If there are still plants or cows alive, show So-So Ending UI
                GoodEndingUI.SetActive(false);
                SoSoEndingUI.SetActive(true);
                BadEndingUI.SetActive(false);
                
            }
            else if (numPlantsAlive == 0 && numCowsAlive == 0)
            {
                // If all plants and cows are dead, show Bad Ending UI
                GoodEndingUI.SetActive(false);
                SoSoEndingUI.SetActive(false);
                BadEndingUI.SetActive(true);
            }

            statsUI.SetActive(false); // Show the stats UI when the game is not active
            PauseMenuUI.SetActive(false); // Show the Pause Menu UI when the game is not active
            pauseButton.SetActive(false); // Hide the pause button when the game is not active
            timer.SetActive(false);

            vignette.intensity.value = 0.55f;
        }
    }

}
