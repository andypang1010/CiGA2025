using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlantInteract : Interactable
{
    GameObject player;
    Rigidbody rb;
    [SerializeField] float throwSpeed = 50;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("PLAYER");
        if (player == null)
        { 
            Debug.LogError("Hello! Plant cannot find Player.");
        }
    }
    public override void React(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.Pickup:
                // get held point from player
                Debug.Log("Yes! Player pickup.");
                GameObject heldpoint = player.GetComponent<PlayerInteraction>().heldPoint;
                transform.SetParent(heldpoint.transform);
                transform.localPosition = Vector3.zero;
                rb.isKinematic = true;
                break;

            case InteractionType.Drop:
                rb.isKinematic = false;
                transform.SetParent(null);
                // GameObject throwPoint = player.GetComponent<PlayerInteraction>().throwPoint;
                // transform.position = throwPoint.transform.position;
                Vector3 faceDir = player.GetComponent<PlayerMovement>().faceDirection;
                rb.AddForce(Vector3.up * 3 + faceDir * throwSpeed, ForceMode.Impulse);
                break;

            case InteractionType.Plant:
                Debug.Log("I am being planted!");
                GameObject grid = player.GetComponent<PlayerInteraction>().closestGrid;
                if (grid == null) { 
                    Debug.LogError("There's no grid nearby but a plant is being planted.");
                    break;
                }
                transform.SetParent(null);
                // snap above the grid
                transform.position = grid.transform.position + Vector3.up * 0.5f;
                rb.isKinematic = true;
                break;

        }
    }
}
