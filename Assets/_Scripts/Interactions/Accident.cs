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
    public bool accidentComplete;
    public bool accidentTimeout;

    public float qteStartTime;
    public float qteTimeLimit = 1f;

    public float accidentStartTime;
    public float accidentDuration = 30f;
    public List<QTEData> qteTimes;

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
        if (Time.time - accidentStartTime >= accidentDuration)
        {
            accidentTimeout = true;
            EndAccident();

            return;
        }

        // If the accident is not active, reset the QTE states and hide the QTE UI
        if (!isAccidentActive)
        {
            ResetAccident();
            EndAccident();
            return;
        }

        accidentUI.SetActive(true); // Show the accident UI

        foreach (var t in qteTimes)
        {
            if (qteTimes.IndexOf(t) == 0)
            {
                accidentComplete |= t.isPressed; // For the first QTE, check if it is pressed
            }
            else
            {
                accidentComplete &= t.isPressed;
            }
        }

        if (accidentComplete)
        {
            EndAccident();
            Destroy(gameObject.GetComponent<Accident>());
        }

        // If the accident is active, check the time and QTE states
        if (Time.time - qteStartTime > qteTimes[qteTimes.Count - 1].time + qteTimeLimit / 2)
        {
            isAccidentActive = false;
            accidentQTE.SetActive(false); // Hide the QTE UI when accident ends
        }

        qteTime.GetComponent<TMP_Text>().text = "Time: " + (Time.time - qteStartTime);

        bool qteMissed = true; // Track whether any QTE was missed

        // Check each QTE event
        foreach (var t in qteTimes)
        {
            // FAIL CONDITION 1: If the QTE time has passed and the player hasn't pressed the key
            if (Time.time - qteStartTime - t.time > qteTimeLimit && !t.isPressed)
            {
                EndAccident();
            }

            // Skip if the QTE was already pressed or if the time window has passed
            if (t.isPressed ||
                (Time.time - qteStartTime - t.time > qteTimeLimit || Time.time - qteStartTime - t.time < -qteTimeLimit))
            {
                continue;
            }

            // If the QTE time window is valid, show the QTE prompt
            if (Time.time - qteStartTime - t.time <= qteTimeLimit / 2 && Time.time - qteStartTime - t.time > -qteTimeLimit / 2)
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
                    qteMissed = false; // Player pressed the key correctly within the window
                    break; // Exit after processing the input
                }
            }
            else
            {
                // Hide the QTE UI if time window is not valid anymore
                accidentQTE.SetActive(false);
            }
        }

        // FAIL CONDITION 2: If the player pressed space when no QTE was active
        if (qteMissed && Input.GetKeyDown(KeyCode.Space))
        {
            EndAccident();
        }
    }

    private void EndAccident()
    {
        isAccidentActive = false; // End the accident if the player missed the QTE or pressed space without a valid QTE
        accidentUI.SetActive(false); // Hide the accident UI
        accidentQTE.SetActive(false); // Hide the QTE UI
    }

    private void ResetAccident()
    {
        foreach (var t in qteTimes)
        {
            t.isPressed = false; // Reset QTE states when accident is not active
        }

        accidentQTE.SetActive(false);
    }
}
