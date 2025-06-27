using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public GameObject heldPoint;

    GameObject heldObject;
    GameObject interactableObject;

    void Update()
    {
        if (interactableObject != null && interactableObject.TryGetComponent(out Interactable interactable))
        {
            if (heldObject == null && Input.GetKeyDown(KeyCode.E))
            {
                heldObject = interactable.gameObject;
                heldObject.transform.SetParent(heldPoint.transform);
                heldObject.transform.localPosition = Vector3.zero;
                Destroy(heldObject.GetComponent<Rigidbody>());
            }

            else if (heldObject != null && Input.GetKeyDown(KeyCode.E))
            {
                heldObject.transform.SetParent(null);
                heldObject.AddComponent<Rigidbody>();
                heldObject = null; // Clear the held object after dropping it
            }
        }

        if (heldObject != null)
        {
            interactableObject = heldObject; // Update interactableObject to the currently held object
        }
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
