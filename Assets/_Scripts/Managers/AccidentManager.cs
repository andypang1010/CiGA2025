using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AccidentManager : MonoBehaviour
{
    [Header("QTEs")]
    public int minBeats = 3;
    public int maxBeats = 7;
    GameObject qteUI;
    GameObject qteBeat;
    GameObject qteTime;
    GameObject qteFailed;
    [SerializeField] List<GameObject> interactables;
    public float accidentChance = 0.6f;
    public float checkInterval = 5f;
    float lastCheckTime = 0f;
    GameObject playerHeldPoint;

    [Header("Chases")]
    public int currentNumPlantsRunning = 0;
    public int maxNumPlantsRunning = 1;
    [SerializeField] List<GameObject> plants;

    string[] qteAccidentables = { "Cow" };

    private void Start()
    {
        playerHeldPoint = GameObject.Find("PLAYER").GetComponent<PlayerInteraction>().heldPoint;

        qteUI = GameObject.Find("Accident UI");
        qteBeat = GameObject.Find("Accident QTE");
        qteTime = GameObject.Find("QTE Time");
        qteFailed = GameObject.Find("QTE Failed");

        qteUI.GetComponent<Image>().enabled = false; // Make sure accident UI is initially hidden
        qteBeat.GetComponent<Image>().enabled = false; // Make sure QTE is initially hidden
        qteTime.GetComponent<TMP_Text>().enabled = false; // Make sure accident UI is initially hidden
        qteFailed.GetComponent<Image>().enabled = false; // Make sure QTE failed UI is initially hidden

        plants = FindObjectsByType<Plant>(FindObjectsSortMode.None).Select(plant => plant.gameObject).ToList();

        foreach(var plant in plants)
        {
            plant.GetComponent<NavMeshAgent>().enabled = false; // Disable all plants initially
        }
    }

    void Update()
    {
        //////////////////////////////// CHASE CODE ////////////////////////////////
        if (currentNumPlantsRunning < maxNumPlantsRunning)
        {
            // Ignore all plants with NavMeshAgent
            plants = FindObjectsByType<Plant>(FindObjectsSortMode.None).Where(plant => !plant.GetComponent<NavMeshAgent>().enabled).
                Select(plant => plant.gameObject).ToList();

            plants[UnityEngine.Random.Range(0, plants.Count - 1)].GetComponent<NavMeshAgent>().enabled = true;
            currentNumPlantsRunning++;
        }

        //////////////////////////////// QTE CODE ////////////////////////////////
        // Only check once per checkInterval seconds and if there is no active accident

        if (Time.time - lastCheckTime < checkInterval) return;

        if (HasActiveAccident()) return;

        lastCheckTime = Time.time;

        print("Last Checked Now @" + lastCheckTime);

        // Check if an accident should occur
        int randomNumber = UnityEngine.Random.Range(0, (int)(1 / accidentChance));
        print("Accident Number: " + randomNumber + " out of range [0, " + (int)(1 / accidentChance) + "]");
        if (randomNumber == 0)
        {
            foreach(var accidentable in qteAccidentables)
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

    bool HasActiveAccident()
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
                hasAccident |= accident.accidentTimeout;
            }
        }

        return hasAccident;
    }
}
