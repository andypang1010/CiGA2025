using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Accident : Interactable
{
    [Serializable]
    public class QTEData
    {
        public float time;
        public bool isPressed;
    }

    public bool isAccidentActive;

    public float accidentStartTime;
    public float qteTimeLimit = 1f;
    public QTEData[] qteTimes;

    GameObject accidentUI;
    GameObject accidentQTE;
    GameObject qteTime;

    public override void React(InteractionType type)
    {
        // React is unused in area-based interaction
        Debug.LogWarning("Accident doesn't use direct interaction anymore.");
    }

    void Start()
    {
        accidentUI = GameObject.Find("ACCIDENT UI");
        accidentQTE = GameObject.Find("Accident QTE");
        qteTime = GameObject.Find("QTE Time");

        accidentQTE.SetActive(false); // Make sure QTE is initially hidden
    }

    void Update()
    {
        accidentUI.SetActive(isAccidentActive);

        // If the accident is not active, reset the QTE states and hide the QTE UI
        if (!isAccidentActive)
        {
            foreach (var t in qteTimes)
            {
                t.isPressed = false; // Reset QTE states when accident is not active
            }

            accidentQTE.SetActive(false);
            return;
        }

        // If the accident is active, check the time and QTE states
        if (Time.time - accidentStartTime > qteTimes[qteTimes.Length - 1].time + qteTimeLimit / 2)
        {
            isAccidentActive = false;
            accidentQTE.SetActive(false); // Hide the QTE UI when accident ends
        }

        qteTime.GetComponent<TMP_Text>().text = "Time: " + (Time.time - accidentStartTime);

        // Check each QTE event
        foreach (var t in qteTimes)
        {
            // Skip if the QTE was already pressed or if the time window has passed
            if (t.isPressed ||
                (Time.time - accidentStartTime - t.time > qteTimeLimit || Time.time - accidentStartTime - t.time < -qteTimeLimit))
            {
                continue;
            }

            // If the QTE time window is valid, show the QTE prompt
            if (Time.time - accidentStartTime - t.time <= qteTimeLimit / 2 && Time.time - accidentStartTime - t.time > -qteTimeLimit / 2)
            {
                if (!accidentQTE.activeSelf) // Show the QTE UI only if it's not already shown
                {
                    accidentQTE.SetActive(true);
                }

                // Check for player input (press space)
                if (Input.GetKeyDown(KeyCode.Space) && !t.isPressed)
                {
                    t.isPressed = true;
                    accidentQTE.SetActive(false); // Hide the QTE UI after pressing the key
                    break; // Exit after processing the input
                }
            }
            else
            {
                // Hide the QTE UI if time window is not valid anymore
                accidentQTE.SetActive(false);
            }
        }
    }
}
