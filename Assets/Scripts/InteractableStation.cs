using UnityEngine;

/// <summary>
/// Base class for any interactable station in the bunker.
/// Inherit from this and implement Interact().
/// </summary>
public abstract class InteractableStation : MonoBehaviour
{
    /// <summary>
    /// Called when the player presses E while this station is the closest in range.
    /// </summary>
    public abstract void Interact(PlayerController player);
}
