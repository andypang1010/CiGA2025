// 1) Define all the possible interaction types in one place:
using UnityEngine;

public enum InteractionType
{
    Pickup,
    Drop,
    Plant,
    Feed,
    Consume,
    Fertilize,
    // …etc
}

// 2) Create an abstract base class (or interface) that your objects implement:
public abstract class Interactable : MonoBehaviour
{
    bool canInteract = true;
    /// <summary>
    /// Tells you if this Interactable can be interacted now.
    /// </summary>
    /// <returns></returns>
    public bool CanInteract() { return canInteract; }
    /// <summary>
    /// Called by your InteractionManager or PlayerController
    /// when the player interacts with this GameObject.
    /// </summary>
    /// <param name="type">What kind of interaction is happening</param>
    public abstract void React(InteractionType type);
}

