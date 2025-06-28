using UnityEngine;

public class Plant : MonoBehaviour
{

    [Header("Read-Only Plant Stats (view-only)")]
    [SerializeField] private float waterLevel = 0f;
    [SerializeField] private float sunLevel = 0f;
    [SerializeField] public bool isDead = false;
    [SerializeField] private bool musicPlayed = false;
    [SerializeField] private int pooCount = 0;
    [SerializeField] private bool isPerfect = false;

    [Header("Growth Settings")]
    public float waterNeeded = 1f;
    public float tooMuchWater = 2f;

    public float sunNeeded = 1f;
    public float tooMuchSun = 2f;

    public int pooNeeded = 1;

    [Header("Growth Rates (units per second)")]
    public float waterRate = 0.2f;
    public float sunRate = 0.2f;

    [Header("Visuals")]
    public Material perfectMaterial;
    private Renderer plantRenderer;

    [Header("Return Logic")]
    public float returnTime = 5f;
    private float returnTimer = 0f;
    private bool isInReturnArea = false;

    [Header("AI Settings")]
    public bool canWander = true;   // Enable/disable wandering for this plant
    [Range(0f, 1f)] public float wanderChance = 0.5f;  // Chance that this plant decides to wander
    public float wanderInterval = 5f;  // How often to pick a new target
    public float wanderDuration = 2f;  // How long to move towards it
    public float wanderRadius = 5f;    // How far from starting point

    private UnityEngine.AI.NavMeshAgent agent;
    private Vector3 startPosition;
    private float wanderTimer = 0f;
    private float wanderDurationTimer = 0f;
    private bool isWandering = false;

    private void Start()
    {
        plantRenderer = GetComponent<Renderer>();
        if (plantRenderer == null)
        {
            plantRenderer = GetComponentInChildren<Renderer>();
        }
        // --- Your existing Start() setup ---
        plantRenderer = GetComponent<Renderer>();
        if (plantRenderer == null) plantRenderer = GetComponentInChildren<Renderer>();

        // NavMeshAgent setup
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        startPosition = transform.position;

        // Random chance to decide whether to wander
        canWander = Random.value <= wanderChance;
    }

    private void Update()
    {
  
        if (isPerfect && isInReturnArea)
        {
            returnTimer -= Time.deltaTime;
            Debug.Log($" Return Timer: {Mathf.Max(0f, returnTimer):F2} seconds remaining");

            if (returnTimer <= 0f)
            {
                Debug.Log("🌿 PLANT SENT");
                Destroy(gameObject);
            }
        }

    
        HandleWandering();
    }

    private void HandleWandering()
    {
        if (!canWander || isDead || isPerfect) return;

        wanderTimer += Time.deltaTime;

        if (!isWandering && wanderTimer >= wanderInterval)
        {
            // Pick a new random destination on the NavMesh
            Vector3 newDestination = GetRandomPoint(startPosition, wanderRadius);
            agent.SetDestination(newDestination);

            isWandering = true;
            wanderDurationTimer = wanderDuration;
            wanderTimer = 0f;
        }

        if (isWandering)
        {
            wanderDurationTimer -= Time.deltaTime;
            if (wanderDurationTimer <= 0f)
            {
                // Stop moving
                agent.ResetPath();
                isWandering = false;
            }
        }
    }

    private Vector3 GetRandomPoint(Vector3 center, float range)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * range;
            randomPos.y = center.y; // keep y level

            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(randomPos, out hit, 2.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return center; // fallback
    }


    private void OnTriggerStay(Collider other)
    {
        if (isDead || isPerfect) return;

        if (other.CompareTag("WaterArea"))
        {
            Water();
        }
        else if (other.CompareTag("SunArea"))
        {
            ExposeToLight();
        }
        else if (other.CompareTag("MusicArea"))
        {
            ListenToMusic();
        }
        else if (other.CompareTag("PooArea"))
        {
            PooPoo();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPerfect && other.CompareTag("ReturnArea"))
        {
            isInReturnArea = true;
             Debug.Log("ENTER");
            returnTimer = returnTime;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPerfect && other.CompareTag("ReturnArea"))
        {
            isInReturnArea = false;
            returnTimer = 0f;
        }
    }

    public void Water()
    {
        waterLevel += waterRate * Time.deltaTime;
        Debug.Log($"Water level: {waterLevel}");

        if (waterLevel >= tooMuchWater)
        {
            Debug.Log("drowned!");
            isDead = true;
        }
        else if (waterLevel >= waterNeeded)
        {
            Debug.Log("watered up!");
            CheckPerfection();
        }
    }

    private void ExposeToLight()
    {
        sunLevel += sunRate * Time.deltaTime;
        Debug.Log($"Sun level: {sunLevel}");

        if (sunLevel >= tooMuchSun)
        {
            Debug.Log("dried!");
            isDead = true;
        }
        else if (sunLevel >= sunNeeded)
        {
            Debug.Log("enough sun!");
            CheckPerfection();
        }
    }

    private void ListenToMusic()
    {
        if (musicPlayed || isPerfect) return;

        musicPlayed = true;
        Debug.Log("nice music!");
        CheckPerfection();
    }

    private void PooPoo()
    {
        if (isPerfect) return;

        pooCount += 1;
        if (pooCount > pooNeeded)
        {
            Debug.Log("too much poo TT!");
        }
        else if (pooCount < pooNeeded)
        {
            Debug.Log("more poo required!");
        }
        else if (pooCount == pooNeeded)
        {
            Debug.Log("poopoo is goodgood");
            CheckPerfection();
        }
    }

    private void CheckPerfection()
    {
        if (isPerfect) return;

        if (
            waterLevel >= waterNeeded &&
            sunLevel >= sunNeeded &&
            pooCount == pooNeeded &&
            musicPlayed
        )
        {
            isPerfect = true;
            Debug.Log("🌟 Plant is perfect!");

            if (perfectMaterial != null && plantRenderer != null)
            {
                plantRenderer.material = perfectMaterial;
            }
        }
    }
}
