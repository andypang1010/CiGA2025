using UnityEngine;

public class Plant : MonoBehaviour
{

    [Header("Read-Only Plant Stats (view-only)")]
    [SerializeField] private float waterLevel = 0f;
    [SerializeField] private float sunLevel = 0f;
    [SerializeField] private float musicLevel = 0f;
    [SerializeField] private float pooCount = 0;
    [SerializeField] public bool isDead = false;
    [SerializeField] public bool isBurnt = false;
    [SerializeField] public bool isShit = false;

    [Header("Plant UI")]
    public GameObject waterUI;
    public GameObject sunUI;
    public GameObject pooUI;
    public GameObject musicUI;
    public GameObject deathUI;
    public GameObject warningUI;

    [Header("Growth Settings")]
    // 0(DEATH) - (POPUP) - minwater - maxwater - (WARNING) - tooMuchWater(DEATH)
    public float initialWater = 2f;
    public float minWater = 1f;
    public float maxWater = 5f;
    public float tooMuchWater = 10f;

    public float initialSun = 2f;
    public float minSun = 1f;
    public float maxSun = 5f;
    public float tooMuchSun = 10f;

    public int initialPoo = 2;
    public int pooNeeded = 1;

    public int initialMusic = 2;
    public int maxmusic = 5;

    [Header("Growth Rates (units per second)")]
    public float waterRate = 1f;
    public float sunRate = 0.5f;
    public float musicRate = 3f;

    [Header("Visuals")]
    public Material perfectMaterial;
    private Renderer plantRenderer;

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
    private float sunDecayInterval = 10f; // how often sun decays
    public float sunDecayAmount = 1f; // how much sun to lose each interval
    private float sunDecayTimer = 0f;
    [SerializeField] private bool isLit = false;

    [Header("Poo Decay Settings")]
    private float pooDecayInterval = 15f; // how often it decays
    public float pooDecayAmount = 1f;
    private float pooDecayTimer = 0f;

    [Header("Music Decay Settings")]
    private float musicDecayInterval = 20f;  // how long before music effect wears off
    public float musicDecayAmount = 1f;
    private float musicDecayTimer = 0f;

    [Header("Water Decay Settings")]
    private float waterDecayInterval = 10f;  // how often water decays
    public float waterDecayAmount = 0.5f;  // how much water to lose each interval
    private float waterDecayTimer = 0f;


    [Header("Audios")]
    public AudioSource warnSound;
    public AudioSource deathSound;


    private void Start()
    {
        plantInitiation();

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

        if (isDead)
        {
            HandleDeath();
        }

        HandleWandering();
        HandleSunDecay();
        HandleWaterDecay();
        HandlePooDecay();
        HandleMusicDecay();

        waterUI.SetActive(waterLevel <= minWater && !isDead);
        sunUI.SetActive(sunLevel <= minSun && !isDead);
        pooUI.SetActive(pooCount <= 3 && !isDead);
        musicUI.SetActive(musicLevel <= 5 && !isDead);
        deathUI.SetActive(isDead);
        
        
    }
    private void plantInitiation()
    {
        waterLevel = initialWater ;
        sunLevel = initialSun ;
        musicLevel = initialMusic;
        pooCount = initialPoo;
    }
    private void CheckDeath()
    {
        //return when dead
        if(isDead) { return; }

        //check water status
        if (waterLevel >= tooMuchWater)
        {
            Debug.Log("drowned!");
            deathSound.Play();
            isDead = true;
        }
        else if (waterLevel <= 0)
        {
            Debug.Log("thirst");
            deathSound.Play();
            isDead = true;
        }

        //check sun status
        if (sunLevel>= tooMuchSun)
        {
            Debug.Log("dried");
            deathSound.Play();
            isDead = true;
            isBurnt = true;
        }
        else if (sunLevel <= 0)
        {
            Debug.Log("withered");
            deathSound.Play();
            isDead = true;
        }

        //check music status
        if(musicLevel <= 0)
        {
            Debug.Log("depressed");
            deathSound.Play();
            isDead = true;
        }

        //check poo status
        if (pooCount > pooNeeded)
        {
            Debug.Log("too much poo TT!");
            deathSound.Play();
            isDead = true;
            isShit = true;
        }
        else if (pooCount <= 0)
        {
            Debug.Log("malnutrition");
            deathSound.Play();
            isDead = true;
        }

    }

    private void HandleDeath()
    {
        if (isBurnt)
        {
            animator.SetTrigger("isBurnt");
        }else if(isShit) {
            animator.SetTrigger("isShit");
        }
        else { 
            animator.SetTrigger("isDead"); 
        }
        
        warningUI.SetActive(false);
        warnSound.Stop();
    }

    private void HandleSunDecay()
    {
        if (sunLevel <= 0) return; // don’t go negative
        {
            if (isDead) return; 

            if (!isLit)
            {
                sunDecayTimer += Time.deltaTime;

                if (sunDecayTimer >= sunDecayInterval)
                {
                    sunLevel -= sunDecayAmount;
                    sunLevel = Mathf.Max(0, sunLevel);

                    // Debug.Log($"☀️ Sun decayed! sunLevel now: {sunLevel}");

                    sunDecayTimer = 0f; // reset timer
                }
            }
            else
            {
                // Reset if the plant is lit again
                sunDecayTimer = 0f;
            }
        }
    }

    private void HandleWaterDecay()
    {
        if (waterLevel <= 0) return; // don’t go negative
        {
            if (isDead) return; 

            waterDecayTimer += Time.deltaTime;

            if (waterDecayTimer >= waterDecayInterval)
            {
                waterLevel -= waterDecayAmount;
                waterLevel = Mathf.Max(0, waterLevel);

               // Debug.Log($"💧 Water decayed! waterLevel now: {waterLevel}");

                waterDecayTimer = 0f; // reset timer
            }
        }
    }

    private void HandlePooDecay()
    {
        if (isDead) return; 

        pooDecayTimer += Time.deltaTime;

        if (pooDecayTimer >= pooDecayInterval)
        {
            pooCount -= pooDecayAmount;
            pooCount = Mathf.Max(0, pooCount); // just in case
            // Debug.Log($"Poo decayed! Current pooCount: {pooCount}");

            pooDecayTimer = 0f; // reset timer
        }
    }
    private void HandleMusicDecay()
    {
        if (isDead) return;

        musicDecayTimer += Time.deltaTime;

        if (musicDecayTimer >= musicDecayInterval)
        {
            musicLevel -= musicDecayAmount;
            musicLevel = Mathf.Max(0f, musicLevel);
            // Debug.Log($"🎵 Music decayed! musicLevel now: {musicLevel}");
            musicDecayTimer = 0f;
        }
    }


    private void HandleWandering()
    {
        if (isDead) return;
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
        if (isDead) return;

        else if (other.CompareTag("SunArea"))
        {
            ExposeToLight();
        }
        else if (other.CompareTag("PooArea"))
        {
            if(pooCount>=pooNeeded)
            {
                warningUI.SetActive(true);
                if (!warnSound.isPlaying)
                {
                    warnSound.Play();
                }
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("SunArea"))
        {
            isLit= true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("SunArea"))
        {
            isLit = false;
            warningUI.SetActive(false);
            if (warnSound.isPlaying)
            {
                warnSound.Stop();
            }
        }
        else if (other.CompareTag("WaterArea"))
        {
            warningUI.SetActive(false) ;
            if (warnSound.isPlaying)
            {
                warnSound.Stop();
            }
        }
        else if (other.CompareTag("PooArea"))
        {
            warningUI.SetActive(false ) ;
            if (warnSound.isPlaying)
            {
                warnSound.Stop();
            }
        }
    }

    public void Water()
    {
        if(isDead) return;
        waterLevel += waterRate;

        if (waterLevel >= maxWater)
        {
            warningUI.SetActive(true);
            if (!warnSound.isPlaying)
            {
                warnSound.Play();
            }
        }
        //Debug.Log($"Water level: {waterLevel}");
    }

    public void PooPoo()
    {
        if (isDead) return;
        pooCount += 1;
    }

    private void ExposeToLight()
    {
        if (isDead) return;
        sunLevel += sunRate * Time.deltaTime;
        if (sunLevel >= maxSun)
        {
            warningUI.SetActive(true);
            if (!warnSound.isPlaying)
            {
                warnSound.Play();
            }
            
            Debug.Log("Burnt");
        }
        // Debug.Log($"Sun level: {sunLevel}");
    }

    public void ListenToMusic()
    {
        if (isDead) return;
        musicLevel = maxmusic;
        //Debug.Log($"🎵 Music level: {musicLevel}");

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


