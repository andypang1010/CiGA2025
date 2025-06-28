using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AccidentManager : MonoBehaviour
{
    public float accidentFrequency = 0.05f;
    public int minBeats = 3;
    public int maxBeats = 7;

    float checkInterval = 5f;
    float lastCheckTime = 0f;

    GameObject playerHeldPoint;

    GameObject accidentUI;
    GameObject accidentQTE;
    GameObject qteTime;

    [SerializeField] List<GameObject> interactables;
    string[] accidentables = { "Cow" };

    private void Start()
    {
        playerHeldPoint = GameObject.Find("PLAYER").GetComponent<PlayerInteraction>().heldPoint;

        accidentUI = GameObject.Find("Accident UI");
        accidentQTE = GameObject.Find("Accident QTE");
        qteTime = GameObject.Find("QTE Time");

        accidentUI.GetComponent<TMP_Text>().enabled = false; // Make sure accident UI is initially hidden
        accidentQTE.GetComponent<Image>().enabled = false; // Make sure QTE is initially hidden
        accidentUI.GetComponent<Image>().enabled = false; // Make sure accident UI is initially hidden
    }

    void Update()
    {
        // Only check once per checkInterval seconds and if there is no active accident
        if (Time.time - lastCheckTime < checkInterval || HasAccident()) return;

        // Check if an accident should occur
        if ((int)UnityEngine.Random.Range(0, 1 / accidentFrequency) == 0)
        {
            lastCheckTime = Time.time;
            foreach(var accidentable in accidentables)
            {
                // Find all objects with the specified tag
                interactables.AddRange(GameObject.FindGameObjectsWithTag(accidentable));
                
            }

            // Remove the held interactable and interactables with accidents if they exist
            if (playerHeldPoint.GetComponentInChildren<Interactable>() != null)
            {
                interactables.Remove(playerHeldPoint.GetComponentInChildren<Interactable>().gameObject);
            }

            List<GameObject> accidentObjects = FindObjectsByType<Accident>(FindObjectsSortMode.None).Select(accident => accident.gameObject).ToList();
            foreach (var accidentObject in accidentObjects)
            {
                interactables.Remove(accidentObject);
            }

            // Create or get the accident component on a random interactable
            Accident accident = interactables[UnityEngine.Random.Range(0, interactables.Count - 1)].AddComponent<Accident>();
            print("ACCIDENT AT: " + accident.gameObject);
            accident.qteTimes = new List<Accident.QTEData>();
            accident.accidentStartTime = Time.time;

            int numBeats = (int)UnityEngine.Random.Range(minBeats, maxBeats);
            float t = 0f;
            
            // Create random beats
            for (int i = 0; i < numBeats; i++)
            {
                float qteTime = (int)UnityEngine.Random.Range(t + 1f, t + 4f);
                Accident.QTEData qteData = new() { time = qteTime, isPressed = false };
                t = qteTime;
                accident.qteTimes.Add(qteData);
            }
        }
    }

    bool HasAccident()
    {
        Accident[] accidents = FindObjectsByType<Accident>(FindObjectsSortMode.None);

        if (accidents.Length == 0)
        {
            return false;
        }

        bool hasAccident = false;

        if (accidents.Length > 0)
        {
            foreach (var accident in accidents)
            {
                hasAccident |= !accident.accidentTimeout;
            }
        }

        return hasAccident;
    }
}
