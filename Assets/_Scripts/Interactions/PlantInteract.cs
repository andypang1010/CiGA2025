using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlantInteract : Interactable
{
    GameObject player;
    Rigidbody rb;
    [SerializeField] float throwSpeed = 50;
    UnityEngine.AI.NavMeshAgent agent;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("PLAYER");
        if (player == null)
        { 
            Debug.LogError("Hello! Plant cannot find Player.");
        }
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent == null)
        {
    Debug.LogWarning("Plant has no NavMeshAgent — wandering won't work.");
        }

        
    }
    public override void React(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.Pickup:
            Debug.Log("Yes! Player pickup.");
            GameObject heldpoint = player.GetComponent<PlayerInteraction>().heldPoint;
            transform.SetParent(heldpoint.transform, false);
            transform.localPosition = Vector3.zero;
            rb.isKinematic = true;
            if (agent != null) agent.enabled = false;
                break;

            case InteractionType.Plant:
                Debug.Log("I am being planted!");
                GameObject grid = player.GetComponent<PlayerInteraction>().closestGrid;
                if (grid == null) { 
                    Debug.LogError("There's no grid nearby but a plant is being planted.");
                    break;
                }
                if (!grid.GetComponent<Grid>().isOccupied)
                {
                    transform.SetParent(null);
                    // snap above the grid
                    transform.position = grid.transform.position + Vector3.up * 0.5f;
                    rb.isKinematic = true;
                    if (agent != null) agent.enabled = true;
                    break;
                }
                else
                {
                    // it is OCCUPIED, GOTO DROP LOGIC
                    Debug.Log("The current grid is OCCUPIED! proceed to dropping.");
                    goto case InteractionType.Drop;
                }
            case InteractionType.Drop:
            rb.isKinematic = false;
            transform.SetParent(null);

            if (agent != null) agent.enabled = true;

            Vector3 faceDir = player.GetComponent<PlayerMovement>().faceDirection;
            rb.AddForce(Vector3.up * 3 + faceDir * throwSpeed, ForceMode.Impulse);
            break;

            case InteractionType.Fertilize:
                GetComponent<Plant>().PooPoo();
                break;

        }
    }
}
