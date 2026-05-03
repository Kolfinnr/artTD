using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LadderZone : MonoBehaviour
{
    private void Reset()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
    }
}
