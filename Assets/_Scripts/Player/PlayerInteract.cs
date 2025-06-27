using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public GameObject heldPoint;

    GameObject heldObject;
    GameObject interactableObject;

    void Update()
    {
        if (interactableObject != null)
        {
            // PLANT
            if (interactableObject.TryGetComponent(out Plant plant))
            {
                if (heldObject == null && Input.GetKeyDown(KeyCode.E))
                {
                    HoldPlant(plant);
                }

                else if (heldObject != null && Input.GetKeyDown(KeyCode.E))
                {
                    ThrowPlant();
                }
            }

            // ACCIDENT
            else if (interactableObject.TryGetComponent(out Accident accident))
            {
                if (heldObject == null && !accident.isAccidentActive && Input.GetKeyDown(KeyCode.E))
                {
                    accident.isAccidentActive = true;
                    accident.accidentStartTime = Time.time;
                }
            }

            else if (interactableObject.TryGetComponent(out WaterSource water))
            {
                if (heldObject == null && Input.GetKey(KeyCode.Space))
                {
                    water.StartWatering();
                }
            }
        }

        if (heldObject != null)
        {
            interactableObject = heldObject; // Update interactableObject to the currently held object
        }
    }

    private void ThrowPlant()
    {
        heldObject.transform.SetParent(null);
        heldObject.GetComponent<Rigidbody>().isKinematic = false;
        heldObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 3 + Vector3.forward * 3, ForceMode.Impulse);
        heldObject = null; // Clear the held object after dropping it
    }

    private void HoldPlant(Interactable interactable)
    {
        heldObject = interactable.gameObject;
        heldObject.transform.SetParent(heldPoint.transform);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            Debug.Log("Interactable object detected: " + other.gameObject.name);
            interactableObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            Debug.Log("Interactable object exited: " + other.gameObject.name);
            interactableObject = null;
        }
    }
}
