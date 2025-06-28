using UnityEngine;

public class ShitGrid : MonoBehaviour
{
    bool hasShit = false;
    Shit currentShit;
    bool hasPlant = false;
    GameObject currentPlant;

    private void Update()
    {
        if (hasShit && hasPlant)
        {
            currentShit.React(InteractionType.Consume);
            currentPlant.GetComponent<PlantInteract>().React(InteractionType.Fertilize);
            hasShit = false;
            currentShit = null;
        }
    }



    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Shit>(out Shit shit))
        {
            Debug.Log("Detected shit 1");
            if (shit.isShitEnabled && !shit.isConsumed)
            {
                Debug.Log("Detected shit 2; consuming shit");
                hasShit = true;
                currentShit = shit;
            }
        }

        if (other.CompareTag("Plant"))
        {
            Debug.Log("Detected Plant");
            hasPlant = true;
            currentPlant = other.gameObject;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Plant"))
        {
            hasPlant = false;
            currentPlant = null;
        }
    }

}
