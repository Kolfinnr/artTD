using System;
using UnityEngine;

public class BunkerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    public int CurrentHealth { get; private set; }

    public event Action<int, int> OnHealthChanged;
    public event Action<int> OnDamaged;
    public event Action OnDepleted;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void ApplyDamage(int amount)
    {
        if (amount <= 0 || CurrentHealth <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnDamaged?.Invoke(CurrentHealth);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth == 0)
        {
            OnDepleted?.Invoke();
        }
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
}
