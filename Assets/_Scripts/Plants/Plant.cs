using UnityEngine;

public class Plant : Interactable
{
    public float waterLevel;

    public override void React(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.Water:
                Water();
                break;
            case InteractionType.ShineLight:
                ExposeToLight();
                break;
            // you can ignore other cases or log an error
            default:
                Debug.LogWarning($"Plant cannot handle interaction {type}");
                break;
        }
    }

    private void Water()
    {
        // waterLevel = Mathf.Min(1f, waterLevel + 0.2f);
        // update visuals, stats, etc.
    }

    private void ExposeToLight()
    {
        // e.g. increase growth rate
    }
}