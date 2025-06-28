using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Shit : Interactable
{
    public float shitExpirationTime = 3;
    public bool isConsumed = false;
    private float originalExpiration;
    public bool isShitEnabled = false;
    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        originalExpiration = shitExpirationTime;
    }

    public override void React(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.Consume:
                // TODO: START playing ANIMATION
                isConsumed = true;
                sr.enabled = false;
                isShitEnabled = false;
                break;
        }
        return;
    }

    private void Update()
    {
        if (isShitEnabled)
        {
            shitExpirationTime -= Time.deltaTime;
        }
        if (shitExpirationTime <= 0.01f)
        {
            isShitEnabled = false;
            sr.enabled = false;
            shitExpirationTime = originalExpiration;
        }
        
    }

    public void InitShit()
    {
        sr.enabled = true;
        isConsumed = false;
        isShitEnabled = true;
        shitExpirationTime = originalExpiration;
    }
}
