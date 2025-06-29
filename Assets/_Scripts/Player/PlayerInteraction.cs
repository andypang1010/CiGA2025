using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public Animator animator;

    [Header("Plant Stats")]
    public GameObject statsUI;
    public TMP_Text plantName;
    public Slider waterSlider;
    public Slider sunlightSlider;
    public Slider musicSlider;
    public Slider shitSlider;

    [Header("Interaction Points")]
    public GameObject heldPoint;
    public GameObject throwPoint;
    public GameObject gridPoint;
    public float interactRange = 0.5f;
    public float gridDetectionRange = 2.5f;
    public GameObject closestGrid;
    public GameObject waterObject;
    public GameObject musicObject;

    [Header("Hold to Play Music")]
    public float musicHoldDuration = 2.0f;  // How long to hold Space
    private float musicHoldTimer = 0.0f;  
    public Slider musicHoldSlider;

    [Header("Hold to Water")]
    public float waterHoldDuration = 2.0f; // seconds to hold before watering
    private float waterHoldTimer = 0.0f;
    public Slider waterHoldSlider;  // assign this in the Inspector

    bool interactPressed;
    GameObject heldObject;
    PlayerMovement movement;


    private void Start()
    {
        //if (heldObject == null) { Debug.LogError("Player should have a held point!!"); }
        // if (throwPoint == null) { Debug.LogError("Player should have a throw point!!"); }
        movement = GetComponent<PlayerMovement>();
    }
    void Update()
    {
    interactPressed = Input.GetKeyDown(KeyCode.E);


    
    // === Update Throw Point if needed ===
    Vector3 faceDir = movement.faceDirection;
    // throwPoint.transform.localPosition = new Vector3(faceDir.x, 0, faceDir.z);

    // === Water logic ===
    if (waterObject != null && waterObject.TryGetComponent(out WaterSource water))
{
    if (heldObject == null)
    {
        if (Input.GetKey(KeyCode.Space))
        {
            waterHoldTimer += Time.deltaTime;

            if (waterHoldTimer >= waterHoldDuration)
            {
                Debug.Log("Hold complete! Starting watering...");
                water.StartWatering();

                // Reset for next cycle if you want repeated watering
                waterHoldTimer = 0.0f;
            }
        }
        else
        {
            // Reset if they release early
            waterHoldTimer = 0.0f;
        }
    }
    else
    {
        waterHoldTimer = 0.0f;
    }
}
else
{
    waterHoldTimer = 0.0f;
}

// === Update water hold UI ===
if (waterHoldSlider != null)
{
    waterHoldSlider.maxValue = waterHoldDuration;
    waterHoldSlider.value = waterHoldTimer;
    waterHoldSlider.gameObject.SetActive(waterObject != null);
}
    // === Hold-to-Play Music logic ===
    if (musicObject != null && musicObject.TryGetComponent(out MusicSource music))
{
    Debug.Log("Inside MusicSource trigger: " + musicObject.name);

    if (heldObject == null)
    {
        if (Input.GetKey(KeyCode.Space))
        {
            musicHoldTimer += Time.deltaTime;
            Debug.Log($"Holding Space: {musicHoldTimer}/{musicHoldDuration}");

            if (musicHoldTimer >= musicHoldDuration)
            {
                Debug.Log("Hold complete! Starting music...");
                music.StartPlayingMusic();

                musicHoldTimer = 0.0f;
            }
        }
        else
        {
            if (musicHoldTimer > 0) Debug.Log("Released Space early, resetting timer.");
            musicHoldTimer = 0.0f;
            
        }
    }
    else
    {
        Debug.Log("Holding object, resetting music hold.");
        musicHoldTimer = 0.0f;
    
    }
}
else
{

    musicHoldTimer = 0.0f;
}

    if (musicHoldSlider != null)
    {
    musicHoldSlider.maxValue = musicHoldDuration;
    musicHoldSlider.value = musicHoldTimer;

    // Optional: hide when not inside source
    musicHoldSlider.gameObject.SetActive(musicObject != null);
    }

    // === Update animator ===
    animator.SetBool("HasHeldObject", heldPoint.transform.childCount > 0);
    animator.SetBool("IsWatering", waterObject != null && (waterHoldTimer > 0 || Input.GetKey(KeyCode.Space)));
    animator.SetBool("IsSinging", musicObject != null && (musicHoldTimer > 0 || Input.GetKey(KeyCode.Space)));

        // === Update Plant Stats UI ===
        if (heldPoint.transform.childCount > 0 && heldObject.TryGetComponent(out Plant plant))
    {
        statsUI.SetActive(true);

        plantName.text = plant.name.Split("(")[0];

        waterSlider.value = plant.GetWaterLevel();
        waterSlider.maxValue = plant.tooMuchWater;

        sunlightSlider.value = plant.GetSunLevel();
        sunlightSlider.maxValue = plant.tooMuchSun;

        musicSlider.value = plant.GetMusicLevel();
            musicSlider.maxValue = plant.maxmusic;

        shitSlider.value = plant.GetPooCount();
            shitSlider.maxValue = plant.pooNeeded;
    }
    else
    {
        statsUI.SetActive(false);
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
                accident.qteStartTime = Time.time;

                return;
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
                Debug.Log("Processing interact type: " + interactionType);
                interactable.React(interactionType);

            }
        }
    }



    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out WaterSource _))
        {
            waterObject = other.gameObject;
        }
        if (other.gameObject.TryGetComponent(out MusicSource _))
    {
        musicObject = other.gameObject;
    }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out WaterSource _))
        {
            waterObject = null;
        }
            if (other.gameObject.TryGetComponent(out MusicSource _))
    {
        musicObject = null;
    }
    }
}
