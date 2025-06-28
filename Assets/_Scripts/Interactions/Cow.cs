using UnityEngine;

public class Cow : Interactable
{
    public GameObject shitPoint;
    public float shitLevel = 0;
    public float shitSpeed = 1;
    public float hungerLevel = 0;
    public float hungerSpeed = 1;

    [SerializeField] private GameObject accidentMark;

    private void Start()
    {
        accidentMark.SetActive(false);
    }

    // update 
    void Update()
    {
        // CHECK ACCIDENT
        if (TryGetComponent<Accident>(out Accident acc))
        {
            if (!acc.accidentTimeout) // TODO: change after andy's update
            {
                accidentMark.SetActive(true);
                // TODO: change to acc. anim
            }
            else
            {
                accidentMark.SetActive(false);
                GoToDie();
            }

            return; // during accident, they cannot starve?
        }
        else
        {
            accidentMark.SetActive(false);
        }


        hungerLevel += Time.deltaTime * hungerSpeed;
        if (hungerLevel >= 100)
        {
            hungerLevel = 100;
            // Trigger cow death or other logic
            // TODO
        }

        shitLevel += Time.deltaTime * shitSpeed;
        if (shitLevel >= 100)
        {
            // handle shit logic
            // TODO
            Poop();

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

    private void Poop()
    {
        // should have shit point
        if (shitPoint == null)
        {
            Debug.LogError("Cow should have space to SHIT! Shit point not there!");
            return; 
        }

        shitPoint.GetComponent<Shit>().InitShit();
    }

    private void GoToDie()
    {
        // TODO
    }

}
