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
    public bool accidentTimeout;
    public bool isQTEFailed;

    public float qteStartTime;
    public float accidentStartTime;
    public float accidentDuration = 45f;
    public List<QTEData> qteTimes;

    float qteTimeLimit = 0.6f;

    GameObject accidentUI;
    GameObject accidentQTE;
    GameObject qteTime;
    GameObject qteFailed;


    public override void React(InteractionType type)
    {
        // React is unused in area-based interaction
        Debug.LogWarning("Accident doesn't use direct interaction anymore.");
    }

    void Start()
    {
        accidentUI = GameObject.Find("Accident UI");
        accidentQTE = GameObject.Find("Accident QTE");
        qteTime = GameObject.Find("QTE Time");
        qteFailed = GameObject.Find("QTE Failed");

        qteTime.GetComponent<TMP_Text>().enabled = false; // Make sure accident UI is initially hidden
        accidentQTE.GetComponent<Image>().enabled = false; // Make sure QTE is initially hidden
        accidentUI.GetComponent<Image>().enabled = false; // Make sure accident UI is initially hidden
        qteFailed.GetComponent<Image>().enabled = false; // Make sure QTE failed UI is initially hidden
    }

    void Update()
    {
        if (Time.time - accidentStartTime >= accidentDuration)
        {
            accidentTimeout = true;
            HideQTE();

            return;
        }

        if (isQTEFailed && !qteFailed.GetComponent<Image>().enabled)
        {
            ShowFailed();
        }

        // If the accident is not active, reset the QTE states and hide the QTE UI
        if (!isAccidentActive)
        {
            ResetAccident();
            HideQTE();
            return;
        }

        // If the accident is complete, hide the QTE and remove the Accident component
        if (CheckAccidentComplete())
        {
            HideQTE();
            Destroy(gameObject.GetComponent<Accident>());
            return;
        }

        // If the accident is active, check the time and QTE states
        if (Time.time - qteStartTime > qteTimes[qteTimes.Count - 1].time + qteTimeLimit / 2)
        {
            HideQTE();
            return;
        }

        ShowQTE();

        bool qteMissed = true; // Track whether any QTE was missed

        // Check each QTE event
        foreach (var t in qteTimes)
        {
            // FAIL CONDITION 1: If the QTE time has passed and the player hasn't pressed the key
            if (Time.time - qteStartTime - t.time > qteTimeLimit && !t.isPressed)
            {
                isQTEFailed = true; // Set the QTE failed state
                HideQTE();
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
                if (!accidentQTE.GetComponent<Image>().enabled) // Show the QTE UI only if it's not already shown
                {
                    accidentQTE.GetComponent<Image>().enabled = true;
                }

                // Check for player input (press space)
                if (Input.GetKeyDown(KeyCode.Space) && !t.isPressed)
                {
                    t.isPressed = true;
                    accidentQTE.GetComponent<Image>().enabled = false; // Hide the QTE UI after pressing the key
                    qteMissed = false; // Player pressed the key correctly within the window
                    break; // Exit after processing the input
                }
            }
            else
            {
                // Hide the QTE UI if time window is not valid anymore
                accidentQTE.GetComponent<Image>().enabled = false;
            }
        }

        // FAIL CONDITION 2: If the player pressed space when no QTE was active
        if (qteMissed && Input.GetKeyDown(KeyCode.Space))
        {
            isQTEFailed = true;
        }
    }

    private bool CheckAccidentComplete()
    {
        bool accidentComplete = false; // Assume the accident is complete unless proven otherwise

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

        return accidentComplete;
    }

    private void ShowQTE()
    {
        isAccidentActive = true; // Ensure the accident is active
        accidentUI.GetComponent<Image>().enabled = true; // Show the accident UI
        qteTime.GetComponent<TMP_Text>().enabled = true; // Show the QTE time text
        qteTime.GetComponent<TMP_Text>().text = (Time.time - qteStartTime).ToString("f3");
    }

    private void HideQTE()
    {
        isAccidentActive = false; // End the accident if the player missed the QTE or pressed space without a valid QTE
        accidentUI.GetComponent<Image>().enabled = false; // Hide the accident UI
        accidentQTE.GetComponent<Image>().enabled = false; // Hide the QTE UI
        qteTime.GetComponent<TMP_Text>().enabled = false; // Hide the QTE time text
        qteFailed.GetComponent<Image>().enabled = false; // Show the QTE failed UI
    }

    private void ShowFailed()
    {
        qteFailed.GetComponent<Image>().enabled = true; // Show the QTE failed UI
        Invoke(nameof(HideFailed), 0.5f); // Hide the QTE failed UI after 0.5 seconds
    }

    private void HideFailed()
    {
        HideQTE(); // Hide the QTE UI as well
    }

    private void ResetAccident()
    {
        foreach (var t in qteTimes)
        {
            t.isPressed = false; // Reset QTE states when accident is not active
        }

        isQTEFailed = false; // Reset QTE failed state
    }
}
