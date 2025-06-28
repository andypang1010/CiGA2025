using UnityEngine;

public class GroundTest : MonoBehaviour
{
    [Tooltip("Tag(s) that count as ground. Can be set in the Inspector.")]
    public string[] groundTags = { "Ground" };

    // Public flag you can read from your character controller
    [HideInInspector]
    public bool isGrounded;

    // Keep track of how many ground contacts we're in,
    // so that overlapping colliders don't prematurely set isGrounded = false.
    private int _groundContacts = 0;

    private void OnTriggerEnter(Collider other)
    {
        // If we hit any collider with a matching tag, count it
        foreach (var tag in groundTags)
        {
            if (other.CompareTag(tag))
            {
                _groundContacts++;
                isGrounded = true;
                return;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When we leave a ground collider, decrement the count
        foreach (var tag in groundTags)
        {
            if (other.CompareTag(tag))
            {
                _groundContacts = Mathf.Max(0, _groundContacts - 1);
                isGrounded = (_groundContacts > 0);
                return;
            }
        }
    }
}
