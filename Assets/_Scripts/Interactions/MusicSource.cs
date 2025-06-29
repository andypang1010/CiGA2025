using System.Collections.Generic;
using UnityEngine;

public class MusicSource : Interactable
{
    public List<Plant> plants = new List<Plant>();

    public override void React(InteractionType type)
    {
        Debug.Log("React called on MusicSource");
        StartPlayingMusic();
    }

    public void StartPlayingMusic()
    {
        Debug.Log("🎵 Music started.");
        foreach (var plant in plants)
        {
            if (!plant.isDead)
            {
                plant.ListenToMusic();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Plant plant) && !plants.Contains(plant))
        {
            plants.Add(plant);
            // Debug.Log($"🎵 Plant added: {plant.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Plant plant))
        {
            plants.Remove(plant);
           //  Debug.Log($"🎵 Plant removed: {plant.name}");
        }
    }
}
