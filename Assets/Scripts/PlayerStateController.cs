using System;
using UnityEngine;

public enum PlayerState
{
    Normal,
    Interacting,
    Aiming,
    Stunned,
    Disabled
}

public class PlayerStateController : MonoBehaviour
{
    public PlayerState CurrentState { get; private set; } = PlayerState.Normal;

    public event Action<PlayerState> OnStateChanged;

    public bool CanMove => CurrentState == PlayerState.Normal || CurrentState == PlayerState.Aiming;
    public bool CanInteract => CurrentState == PlayerState.Normal;
    public bool CanAim => CurrentState == PlayerState.Aiming;

    public void SetState(PlayerState nextState)
    {
        if (CurrentState == nextState)
        {
            return;
        }

        CurrentState = nextState;
        OnStateChanged?.Invoke(nextState);
    }
}
