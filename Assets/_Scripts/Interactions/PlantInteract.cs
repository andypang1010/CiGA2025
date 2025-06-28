using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlantInteract : Interactable
{
    GameObject player;
    Rigidbody rb;
    [SerializeField] float throwSpeed = 50;
    [SerializeField] GameObject groundCheck;
    UnityEngine.AI.NavMeshAgent agent;
    private bool isHoldingThing = false;

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
                isHoldingThing = true;
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
                    isHoldingThing = false;
                    ResetScale();
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
                Vector3 faceDir = player.GetComponent<PlayerMovement>().faceDirection;
                rb.AddForce(Vector3.up * 3 + faceDir * throwSpeed, ForceMode.Impulse);
                isHoldingThing = false;
                ResetScale();
                // if (agent != null) agent.enabled = true;
                if (agent != null)
                {
                    // start coroutine to re-enable agent after landing
                    StartCoroutine(ReenableAgentWhenGrounded());
                }

                // if (agent != null) agent.enabled = true; This line will immediately activate the agent, so that we might be carried away!
                break;

            case InteractionType.Fertilize:
                GetComponent<Plant>().PooPoo();
                break;

        }
    }

    private IEnumerator ReenableAgentWhenGrounded()
    {
        // wait until the next fixed update to allow physics to settle
        yield return new WaitForFixedUpdate();
        // optionally, wait a bit longer if needed:
        yield return new WaitForSeconds(2f);

        bool isGrounded = false;
        if (groundCheck != null)
        {
            isGrounded = groundCheck.GetComponent<GroundTest>().isGrounded;
        }

        if (agent != null && isGrounded)
        {
            Debug.Log("Plant " + gameObject.name + " is grounded, turning agent on.");
            agent.Warp(transform.position);
            agent.ResetPath();
            agent.enabled = true;
        }
    }

    private void ResetScale()
    {
        sbyte scale = 1;
        transform.localScale = new Vector3 (scale, scale, scale);
    }
}
