using System;
using UnityEngine;

public enum GameState
{
    Prep,
    Aiming,
    Resolving,
    Stunned,
    GameOver
}

public class GameLoopManager : MonoBehaviour
{
    [SerializeField] private EnemyArtilleryTimer enemyArtilleryTimer;
    [SerializeField] private BunkerHealth bunkerHealth;
    [SerializeField] private PlayerStateController playerStateController;
    [SerializeField] private float stunDurationSeconds = 2.5f;

    public GameState CurrentState { get; private set; } = GameState.Prep;

    public event Action<GameState> OnStateChanged;

    private float stunTimer;

    private void Awake()
    {
        if (enemyArtilleryTimer == null)
        {
            enemyArtilleryTimer = FindFirstObjectByType<EnemyArtilleryTimer>();
        }

        if (bunkerHealth == null)
        {
            bunkerHealth = FindFirstObjectByType<BunkerHealth>();
        }

        if (playerStateController == null)
        {
            playerStateController = FindFirstObjectByType<PlayerStateController>();
        }
    }

    private void OnEnable()
    {
        if (enemyArtilleryTimer != null)
        {
            enemyArtilleryTimer.OnTimeout += HandleEnemyTimeout;
        }

        if (bunkerHealth != null)
        {
            bunkerHealth.OnDepleted += HandleBunkerDepleted;
        }
    }

    private void OnDisable()
    {
        if (enemyArtilleryTimer != null)
        {
            enemyArtilleryTimer.OnTimeout -= HandleEnemyTimeout;
        }

        if (bunkerHealth != null)
        {
            bunkerHealth.OnDepleted -= HandleBunkerDepleted;
        }
    }

    private void Start()
    {
        SetState(GameState.Prep);
    }

    private void Update()
    {
        if (CurrentState != GameState.Stunned)
        {
            return;
        }

        stunTimer -= Time.deltaTime;

        if (stunTimer <= 0f)
        {
            SetState(GameState.Prep);
        }
    }

    public void SetState(GameState nextState)
    {
        if (CurrentState == GameState.GameOver && nextState != GameState.GameOver)
        {
            return;
        }

        CurrentState = nextState;
        ApplyPlayerStateForGameState(nextState);
        OnStateChanged?.Invoke(nextState);
    }

    private void HandleEnemyTimeout()
    {
        if (CurrentState == GameState.GameOver)
        {
            return;
        }

        bunkerHealth.ApplyDamage(1);

        if (CurrentState != GameState.GameOver)
        {
            stunTimer = stunDurationSeconds;
            SetState(GameState.Stunned);
        }
    }

    private void HandleBunkerDepleted()
    {
        SetState(GameState.GameOver);
    }

    private void ApplyPlayerStateForGameState(GameState state)
    {
        if (playerStateController == null)
        {
            return;
        }

        switch (state)
        {
            case GameState.Prep:
                playerStateController.SetState(PlayerState.Normal);
                break;
            case GameState.Aiming:
                playerStateController.SetState(PlayerState.Aiming);
                break;
            case GameState.Resolving:
                playerStateController.SetState(PlayerState.Disabled);
                break;
            case GameState.Stunned:
                playerStateController.SetState(PlayerState.Stunned);
                break;
            case GameState.GameOver:
                playerStateController.SetState(PlayerState.Disabled);
                break;
        }
    }
}
