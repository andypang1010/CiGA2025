using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Shit : Interactable
{
    public float shitExpirationTime = 3;
    private float originalExpiration;
    private bool isShitEnabled = false;
    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        originalExpiration = shitExpirationTime;
    }

    public override void React(InteractionType type)
    {
        throw new System.NotImplementedException();
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
        isShitEnabled = true;
    }
}
