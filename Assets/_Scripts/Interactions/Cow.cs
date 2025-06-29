using UnityEngine;

public class Cow : Interactable
{
    public bool testDeath = false;
    public bool testDepressed = false;
    public bool isDead = false;
    public GameObject shitPoint;
    public float shitLevel = 0;
    public float shitTime = 10;
    public float hungerLevel = 0;
    public float hungerSpeed = 1;
    [SerializeField] Animator animator;
    [SerializeField] GameObject sprite;
    [SerializeField] Sprite deadSprite;
    [SerializeField] private GameObject accidentMark;
    private bool isDepressed = false;

    private void Start()
    {
        accidentMark.SetActive(false);
    }

    // update 
    void Update()
    {
        if (isDead) return;
        if (testDeath)
        {
            GoToDie();
            testDeath = false;
            accidentMark.SetActive(true);
            return;
        }
        if (testDepressed)
        {
            animator.SetTrigger("Depressed");
            accidentMark.SetActive(true);
            return;
        }

        // CHECK ACCIDENT
        if (TryGetComponent<Accident>(out Accident acc))
        {
            if (!acc.accidentTimeout) // TODO: change after andy's update
            {
                accidentMark.SetActive(true);
                isDepressed = true;
                // TODO: change to acc. anim
                animator.SetTrigger("Depressed");
            }
            else
            {
                accidentMark.SetActive(false);
                GoToDie();
                isDepressed = false;
            }

            return; // during accident, they cannot starve?
        }
        else
        {
            accidentMark.SetActive(false);
            if (isDepressed)
            {
                isDepressed = false;
                animator.SetTrigger("Resolved");
            }
        }


        hungerLevel += Time.deltaTime * hungerSpeed;
        if (hungerLevel >= 100)
        {
            hungerLevel = 100;
            // Trigger cow death or other logic
            // TODO
        }

        shitLevel += Time.deltaTime;
        if (shitLevel >= shitTime)
        {
            // handle shit logic
            // TODO
            animator.SetTrigger("Poop");

            shitLevel = 0;
        }
    }

    public override void React(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.Feed:
                // handle feed logic
                // TODO
                break;
        }
    }

    public void Poop()
    {
        // should have shit point
        if (shitPoint == null)
        {
            Debug.LogError("Cow should have space to SHIT! Shit point not there!");
            return; 
        }

        shitPoint.GetComponent<Shit>().animator.SetTrigger("Poop");
    }

    private void GoToDie()
    {
        // TODO
        animator.SetTrigger("Die");
        sprite.GetComponent<SpriteRenderer>().sprite = deadSprite;
        isDead = true;
    }

}
