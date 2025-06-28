using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool isOccupied = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Plant"))
        {
            isOccupied = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Plant"))
        {
            isOccupied = false;
        }
    }
}
