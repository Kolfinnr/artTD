using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyTarget : MonoBehaviour
{
    public event Action OnHit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Projectile"))
        {
            return;
        }

        OnHit?.Invoke();
    }
}
