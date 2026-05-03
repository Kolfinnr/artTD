using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles player movement and station interactions.
/// Attach this to the player GameObject.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;

    private Rigidbody2D rb;
    private Vector2 movementInput;

    // Stores all stations currently inside the player's trigger area.
    private readonly List<InteractableStation> stationsInRange = new List<InteractableStation>();

    // Closest station from the list above.
    private InteractableStation nearestStation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        ReadMovementInput();
        HandleInteractInput();
        UpdateNearestStation();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movementInput * moveSpeed;
    }

    /// <summary>
    /// Reads WASD/Arrow input.
    /// </summary>
    private void ReadMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        movementInput = new Vector2(horizontal, vertical).normalized;
    }

    /// <summary>
    /// Press E to interact with the closest station in range.
    /// </summary>
    private void HandleInteractInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && nearestStation != null)
        {
            nearestStation.Interact(this);
        }
    }

    /// <summary>
    /// Updates nearestStation by checking distance to each station in range.
    /// </summary>
    private void UpdateNearestStation()
    {
        nearestStation = null;
        float closestDistance = float.MaxValue;

        for (int i = stationsInRange.Count - 1; i >= 0; i--)
        {
            // Clean up null references in case an object was destroyed.
            if (stationsInRange[i] == null)
            {
                stationsInRange.RemoveAt(i);
                continue;
            }

            float distance = Vector2.Distance(transform.position, stationsInRange[i].transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestStation = stationsInRange[i];
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        InteractableStation station = other.GetComponent<InteractableStation>();

        if (station != null && !stationsInRange.Contains(station))
        {
            stationsInRange.Add(station);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        InteractableStation station = other.GetComponent<InteractableStation>();

        if (station != null)
        {
            stationsInRange.Remove(station);

            if (nearestStation == station)
            {
                nearestStation = null;
            }
        }
    }
}
