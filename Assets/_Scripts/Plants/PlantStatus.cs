using UnityEngine;

/// <summary>
/// Determine if the plant is rooted or unrooted.
/// </summary>
public class PlantStatus: MonoBehaviour
{
    /// <summary>
    /// The plant is rooted in the ground.
    /// </summary>
    public bool isRooted;
    Rigidbody rb;


    void Start()
    {
        isRooted = true;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // check status; if is rooted, then disable any moving
        if (isRooted && !rb.isKinematic)
        {
            rb.isKinematic = true;
        } else if (!isRooted && rb.isKinematic) {
        
            rb.isKinematic = false;
        }
    }
    
}
