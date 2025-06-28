using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AccidentManager : MonoBehaviour
{
    public float accidentFrequency = 0.05f;
    public int minBeats = 3;
    public int maxBeats = 8;
    public float maxQTEDuration = 10;

    float checkInterval = 1f;
    float lastCheckTime = 0f;

    [SerializeField] List<Interactable> interactables;

    void Update()
    {
        // Only check once per checkInterval seconds
        if (Time.time - lastCheckTime < checkInterval) return;

        // Check if an accident should occur
        if (Random.Range(0, 1 / accidentFrequency) == 0)
        {
            lastCheckTime = Time.time;
            interactables = FindObjectsByType<Interactable>(FindObjectsSortMode.None).ToListPooled();

            // Remove the held interactable if it exists
            interactables.Remove(GameObject.Find("Player").GetComponent<PlayerInteraction>().heldPoint.GetComponentInChildren<Interactable>());

            // Create or get the accident component on a random interactable
            Accident accident = interactables[Random.Range(0, interactables.Count - 1)].GetOrAddComponent<Accident>();
            print("ACCIDENT AT: " + accident.gameObject);
            accident.qteTimes.Clear();

            int numBeats = Random.Range(minBeats, maxBeats);
            float t = 0f;
            
            // Create random beats
            for (int i = 0; i < numBeats; i++)
            {
                float qteTime = Random.Range(t + 1f, maxQTEDuration);
                Accident.QTEData qteData = new() { time = qteTime, isPressed = false };
                t = qteTime;
                accident.qteTimes.Add(qteData);
            }
        }
    }
}
