using UnityEngine;

public class Plant : MonoBehaviour
{

    [Header("Read-Only Plant Stats (view-only)")]
    [SerializeField] private float waterLevel = 0f;
    [SerializeField] private float sunLevel = 0f;
    [SerializeField] private float musicLevel = 0f;
    [SerializeField] private float pooCount = 0;
    [SerializeField] private bool isPerfect = false;
    [SerializeField] public bool isDead = false;

    [Header("Plant UI")]
    public GameObject waterUI;
    public GameObject sunUI;
    public GameObject pooUI;
    public GameObject musicUI;

    [Header("Growth Settings")]
    public float waterNeeded = 1f;
    public float tooMuchWater = 2f;

    public float sunNeeded = 1f;
    public float tooMuchSun = 2f;

    public int pooNeeded = 1;
    public int tooMuchPoo = 5;

    public int musicNeeded = 1;   
    public int tooMuchMusic = 2;  

    [Header("Growth Rates (units per second)")]
    public float waterRate = 1f;
    public float sunRate = 0.5f;
    public float sunDecayRate = 0.25f;
    public float musicRate = 0.2f;  

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

    [Header("Animation Settings")]
    public Animator animator;

    [Header("Sun Decay Settings")]
    [SerializeField] private bool isLit = false;

    [Header("Poo Decay Settings")]
    public float pooDecayInterval = 5f; // how often it decays
    private float pooDecayTimer = 0f;

    [Header("Music Decay Settings")]
    public float musicDecayInterval = 5f;  // how long before music effect wears off
    private float musicDecayTimer = 0f;


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

        CheckDeath();
  
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

        if(isDead)
        {
            animator.SetTrigger("isDead");
        }
    
        HandleWandering();
        HandleSunDecay();
        HandleWaterDecay();
        HandlePooDecay();
        HandleMusicDecay();

        waterUI.SetActive(tooMuchWater - waterLevel <= tooMuchWater / 5 || waterLevel - 0 <= tooMuchWater / 5);
        sunUI.SetActive(tooMuchSun - sunLevel <= tooMuchSun / 10 || sunLevel - 0 <= tooMuchSun / 10);
        pooUI.SetActive(pooCount >= pooNeeded || pooCount <= 0);
        musicUI.SetActive(tooMuchMusic - musicLevel <= tooMuchMusic / 5 || musicLevel - 0 <= tooMuchMusic / 5);
    }

    private void CheckDeath()
    {

    }

    private void HandleSunDecay()
    {
        if (sunLevel <= 0) return; // don't go negative

        //decay if not in SunArea
        if (!isLit)
        {
            sunLevel -= sunDecayRate*Time.deltaTime;
        }
    }

    private void HandleWaterDecay()
    {
        if (waterLevel <= 0) return; // don't go negative

        pooDecayTimer += Time.deltaTime;

        if (pooDecayTimer >= pooDecayInterval)
        {
            pooCount--;
            pooCount = Mathf.Max(0, pooCount); // just in case
            Debug.Log($"Poo decayed! Current pooCount: {pooCount}");

            pooDecayTimer = 0f; // reset timer
        }
    }

    private void HandlePooDecay()
{
    if (pooCount <= 0) return; // don't go negative

    pooDecayTimer += Time.deltaTime;

    if (pooDecayTimer >= pooDecayInterval)
    {
        pooCount--;
        pooCount = Mathf.Max(0, pooCount); // just in case
        Debug.Log($"Poo decayed! Current pooCount: {pooCount}");

        pooDecayTimer = 0f; // reset timer
    }
}
    private void HandleMusicDecay()
{
    if (musicLevel <= 0f) return;

    musicDecayTimer += Time.deltaTime;

    if (musicDecayTimer >= musicDecayInterval)
    {
        musicLevel--;
        musicLevel = Mathf.Max(0f, musicLevel);
        Debug.Log($"🎵 Music decayed! musicLevel now: {musicLevel}");
        musicDecayTimer = 0f;
    }
}


    private void HandleWandering()
{
    if (isDead || isPerfect) return;
    if (agent == null || !agent.enabled) return; 

    wanderTimer += Time.deltaTime;

    if (!isWandering && wanderTimer >= wanderInterval)
    {
        // Roll the chance each time
        bool willWanderThisTime = Random.value <= wanderChance;

        if (willWanderThisTime)
        {
            Vector3 newDestination = GetRandomPoint(startPosition, wanderRadius);
            agent.SetDestination(newDestination);

            isWandering = true;
            wanderDurationTimer = wanderDuration;
        }

        wanderTimer = 0f; // Reset either way
    }

    if (isWandering)
    {
        wanderDurationTimer -= Time.deltaTime;
        if (wanderDurationTimer <= 0f)
        {
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

        else if (other.CompareTag("SunArea"))
        {
            ExposeToLight();
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

        if (other.CompareTag("SunArea"))
        {
            isLit= true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPerfect && other.CompareTag("ReturnArea"))
        {
            isInReturnArea = false;
            returnTimer = 0f;
        }

        if (other.CompareTag("SunArea"))
        {
            isLit = false;
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

    public void PooPoo()
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

    public void ListenToMusic()
{
    if (isPerfect) return;

    musicLevel += musicRate * Time.deltaTime;
    Debug.Log($"🎵 Music level: {musicLevel}");

    if (musicLevel >= tooMuchMusic)
    {
        Debug.Log("🎵 Too much music! Plant is overwhelmed!");
        isDead = true;
    }
    else if (musicLevel >= musicNeeded)
    {
        Debug.Log("🎵 Enough music!");
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
            musicLevel >= musicNeeded
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

    public float GetWaterLevel()
    {
        return waterLevel;
    }

    public float GetSunLevel()
    {
        return sunLevel;
    }

    public float GetMusicLevel()
    {
        return musicLevel;
    }

    public float GetPooCount()
    {
        return pooCount;
    }
}
