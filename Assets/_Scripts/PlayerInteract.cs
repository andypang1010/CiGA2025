using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public GameObject heldObject;
    public GameObject heldPoint;

    public GameObject interactableObject;

    void Update()
    {
        if (interactableObject != null && interactableObject.TryGetComponent(out Interactable interactable))
        {
            if (heldObject == null && Input.GetKeyDown(KeyCode.E))
            {
                heldObject = interactable.gameObject;
                heldObject.transform.SetParent(heldPoint.transform);
                heldObject.transform.localPosition = Vector3.zero;
                heldObject.GetComponent<Rigidbody>().isKinematic = true;

                interactableObject = null; // Clear the interactable object after picking it up
            }
            else
            {
                Debug.Log("Already holding an object.");
            }
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
