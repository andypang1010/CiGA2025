using UnityEngine;

public class Plant : Interactable
{

    public override void React(InteractionType type)
{
    // React is unused in area-based interaction
    Debug.LogWarning("Plant doesn't use direct interaction anymore.");
}

    [Header("Read-Only Plant Stats (view-only)")]
    [SerializeField] private float waterLevel = 0f;
    [SerializeField] private float sunLevel = 0f;
    [SerializeField] private bool isDead = false;
    [SerializeField] private float musicLevel = 0f;

    [Header("Growth Settings")]
    public float waterNeeded = 1f;
    public float tooMuchWater = 2f;

    public float sunNeeded = 1f;
    public float tooMuchSun = 2f;

    public float musicNeeded = 1f;
    public float tooMuchMusic = 2f;

    [Header("Trigger Areas")]
    public GameObject sunArea;
    public GameObject waterArea;
    public GameObject musicArea;


    private void OnTriggerStay(Collider other)
    {
        if (isDead) return;

        if (other.gameObject == waterArea)
        {
            Water();
        }
        else if (other.gameObject == sunArea)
        {
            ExposeToLight();
        }
        else if (other.gameObject == musicArea)
        {
            ListenToMusic();
        }
    }

    private void Water()
    {
        waterLevel += 0.2f;
        Debug.Log($"Water level: {waterLevel}");

        if (waterLevel >= tooMuchWater)
        {
            Debug.Log("drowned!");
            isDead = true;
        }
        else if (waterLevel >= waterNeeded)
        {
            Debug.Log("watered up!");

        }
        // waterLevel = Mathf.Min(1f, waterLevel + 0.2f);
        // update visuals, stats, etc.
    }

    private void ExposeToLight()
    {
        sunLevel += 0.2f;
        Debug.Log($"Sun level: {sunLevel}");

        if (sunLevel >= tooMuchSun)
        {
            Debug.Log("dried!");
            isDead = true;
        }
        else if (sunLevel >= sunNeeded)
        {
            Debug.Log("enough sun!");
        }
        // e.g. increase growth rate
    }


    private void ListenToMusic()
    {
        musicLevel += 0.2f * Time.deltaTime;
        Debug.Log($"Music level: {musicLevel}");

        if (musicLevel >= tooMuchMusic)
        {
            Debug.Log("overstimulated!");
            isDead = true;
        }
        else if (musicLevel >= musicNeeded)
        {
            Debug.Log("grooving!");
        }
    }
}
