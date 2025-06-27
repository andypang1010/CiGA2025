using UnityEditor.Experimental.GraphView;
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
    [SerializeField] private bool musicPlayed = false;
    [SerializeField] private int pooCount = 0;

    [Header("Growth Settings")]
    public float waterNeeded = 1f;
    public float tooMuchWater = 2f;

    public float sunNeeded = 1f;
    public float tooMuchSun = 2f;

    public int pooNeeded = 1;

    [Header("Growth Rates (units per second)")]
    public float waterRate = 0.2f;
    public float sunRate = 0.2f;
    //public float musicRate = 0.2f;


    private void OnTriggerStay(Collider other)
    {
        if (isDead) return;

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

    private void Water()
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

        }
        // waterLevel = Mathf.Min(1f, waterLevel + 0.2f);
        // update visuals, stats, etc.
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
        }
        // e.g. increase growth rate
    }


    private void ListenToMusic()
    {
        musicPlayed = true;
        Debug.Log("nice music!");
    }

    private void PooPoo()
    {
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
        }


    }
}
