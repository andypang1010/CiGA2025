using System.Linq;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject heldPoint;
    /// <summary>
    /// This variable is currently UNUSED. originally, it's for snapping the held object's position before throwing.
    /// </summary>
    public GameObject throwPoint;
    public GameObject gridPoint;
    public float interactRange = 0.5f;
    public float gridDetectionRange = 2.5f;
    public GameObject closestGrid;
    bool interactPressed;
    GameObject heldObject;
    PlayerMovement movement;
    public GameObject waterObject;

    private void Start()
    {
        if (heldObject == null) { Debug.LogError("Player should have a held point!!"); }
        // if (throwPoint == null) { Debug.LogError("Player should have a throw point!!"); }
        movement = GetComponent<PlayerMovement>();
    }
    void Update()
    {
        interactPressed = Input.GetKeyDown(KeyCode.E);
        Debug.Log("Interact Pressed: " + interactPressed);

        // update throwpoint to always stay in front of player
        Vector3 faceDir = movement.faceDirection;


        // throwPoint.transform.localPosition = new Vector3(faceDir.x, 0, faceDir.z);

        // Water logic; IT IS NOT AN INTERACTION. Should not belong here. but i should make it work first.
        if (waterObject != null && waterObject.TryGetComponent(out WaterSource water))
        {
            if (heldObject == null && Input.GetKey(KeyCode.Space))
            {
                water.StartWatering();
            }
        }
    }

    private void FixedUpdate()
    {


        if (!interactPressed) { return; }
        if (heldObject)
        {
            if (heldObject.GetComponent<Plant>() != null) {
                // check if there's a grid nearby
                GameObject grid = FindClosestPlantGrid();
                if (grid != null)
                {
                    Debug.Log("initiating a plant action");
                    closestGrid = grid;
                    Interact(heldObject, InteractionType.Plant);
                }
                else
                {
                    Interact(heldObject, InteractionType.Drop);
                }
            }
            else
            {
                Interact(heldObject, InteractionType.Drop);
            }
            heldObject = null;
            return;
        }

        GameObject closestInteractable = null;
        closestInteractable = FindClosestObjectInRange();
        if (closestInteractable == null)
        {
            Debug.Log("No interactable object found within range.");
            return;
        }

        // get the interaction type
        InteractionType interactionType = InteractionType.Pickup;
        // check held object
        if (heldObject) { interactionType = InteractionType.Drop; }
        // check accident
        else if (closestInteractable.TryGetComponent(out Accident accident))
        {
            if (!accident.isAccidentActive)
            {
                accident.isAccidentActive = true;
                accident.accidentStartTime = Time.time;
            }
        }
        // MORE LOGIC： 

        Interact(closestInteractable, interactionType);

        // post-processing for specific interactions
        if (interactionType == InteractionType.Pickup)
        {
            heldObject = closestInteractable;
        }
    }

    private GameObject FindClosestPlantGrid()
    {
        GameObject[] grids = Physics.OverlapSphere(gridPoint.transform.position,
            gridDetectionRange, LayerMask.GetMask("Grid")).Select(collider => collider.gameObject).ToArray();
        if (grids.Length == 0) return null;

        float closestDistance = Mathf.Infinity;
        GameObject closestObject = null;
        foreach (GameObject grid in grids)
        {
            float currentDistance = Vector3.Distance(transform.position, grid.transform.position);
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                closestObject = grid;
            }
        }
        return closestObject;
    }
    
    private GameObject FindClosestObjectInRange()
    {
        GameObject[] interactables = Physics.OverlapSphere(transform.position,
            interactRange, LayerMask.GetMask("Interactable")).Select(collider => collider.gameObject).ToArray();

        if (interactables.Length == 0)
        {
            return null;
        }

        float closestDistance = Mathf.Infinity;
        GameObject closestObject = null;
        foreach (GameObject interactable in interactables)
        {
            float currentDistance = Vector3.Distance(transform.position, interactable.transform.position);
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                closestObject = interactable;
            }
        }
        return closestObject;
    }

    private static void Interact(GameObject closestInteractable, InteractionType interactionType)
    {
        // fetch **all** Interactable components on that GameObject
        Interactable[] allInteractables = closestInteractable.GetComponents<Interactable>();

        foreach (var interactable in allInteractables)
        {
            // only invoke on the ones that are ready
            if (interactable != null && interactable.CanInteract())
            {
                Debug.Log("Yes! Can Interact!");
                interactable.React(interactionType);

            }
        }
    }



    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Interactable") && other.gameObject.TryGetComponent(out WaterSource _))
        {
            waterObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            waterObject = null;
        }
    }
}
