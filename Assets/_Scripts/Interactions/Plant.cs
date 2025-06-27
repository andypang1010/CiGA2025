using UnityEngine;

public class Plant : Interactable
{
    public override void React(InteractionType type)
    {
        Debug.LogWarning("Plant doesn't use direct interaction anymore.");
    }

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

    private void Start()
    {
        plantRenderer = GetComponent<Renderer>();
        if (plantRenderer == null)
        {
            plantRenderer = GetComponentInChildren<Renderer>();
        }
    }

    private void Update()
{
    if (isPerfect && isInReturnArea)
    {
        returnTimer -= Time.deltaTime;

        Debug.Log($"⏳ Return Timer: {Mathf.Max(0f, returnTimer):F2} seconds remaining");

        if (returnTimer <= 0f)
        {
            Debug.Log("🌿 PLANT SENT");
            Destroy(gameObject);
        }
    }
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
            returnTimer = returnTimer;
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
