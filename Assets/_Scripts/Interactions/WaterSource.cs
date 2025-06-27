using System.Collections.Generic;
using UnityEngine;

public class WaterSource : Interactable
{
    public List<Plant> plants = new List<Plant>();
    public override void React(InteractionType type)
    {
        throw new System.NotImplementedException();
    }

    public void StartWatering()
    {
        // Logic to start watering
        Debug.Log("Watering started.");
        foreach (var plant in plants)
        {
            if (!plant.isDead)
            {
                plant.Water();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Plant plant))
        {
            plants.Add(plant);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Plant plant))
        {
            plants.Remove(plant);
        }
    }
}
