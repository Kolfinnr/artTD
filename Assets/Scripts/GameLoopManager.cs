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
    [Header("Dependencies")]
    [SerializeField] private EnemyArtilleryTimer enemyArtilleryTimer;
    [SerializeField] private BunkerHealth bunkerHealth;
    [SerializeField] private PlayerStateController playerStateController;

    [Header("Round / Stun")]
    [SerializeField] private int requiredTasksForPrep = 3;
    [SerializeField] private float stunDurationSeconds = 2.5f;

    public GameState CurrentState { get; private set; } = GameState.Prep;
    public int CompletedPrepTasks { get; private set; }

    public event Action<GameState> OnStateChanged;
    public event Action<int, int> OnPrepProgressChanged;

    private float stunTimer;
    private GameState stateBeforeStun = GameState.Prep;

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
        EnterPrepState(resetPrepProgress: true);
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
            ExitStunnedState();
        }
    }

    public void ReportPrepTaskCompleted()
    {
        if (CurrentState != GameState.Prep || requiredTasksForPrep <= 0)
        {
            return;
        }

        CompletedPrepTasks = Mathf.Min(CompletedPrepTasks + 1, requiredTasksForPrep);
        OnPrepProgressChanged?.Invoke(CompletedPrepTasks, requiredTasksForPrep);

        if (CompletedPrepTasks >= requiredTasksForPrep)
        {
            SetState(GameState.Aiming);
        }
    }

    public void ReportPlayerFired()
    {
        if (CurrentState != GameState.Aiming)
        {
            return;
        }

        SetState(GameState.Resolving);
    }

    public void ReportShotResolved()
    {
        if (CurrentState != GameState.Resolving)
        {
            return;
        }

        EnterPrepState(resetPrepProgress: true);
    }

    public void ReportEnemyDestroyed()
    {
        SetState(GameState.GameOver);
    }

    private void EnterPrepState(bool resetPrepProgress)
    {
        if (resetPrepProgress)
        {
            CompletedPrepTasks = 0;
            OnPrepProgressChanged?.Invoke(CompletedPrepTasks, requiredTasksForPrep);
        }

        SetState(GameState.Prep);
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
            EnterStunnedState();
        }
    }

    private void EnterStunnedState()
    {
        if (CurrentState != GameState.Stunned)
        {
            stateBeforeStun = CurrentState;
        }

        stunTimer = stunDurationSeconds;
        SetState(GameState.Stunned);
    }

    private void ExitStunnedState()
    {
        if (CurrentState != GameState.Stunned)
        {
            return;
        }

        GameState resumeState = stateBeforeStun;

        if (resumeState == GameState.GameOver || resumeState == GameState.Stunned)
        {
            resumeState = GameState.Prep;
        }

        SetState(resumeState);
    }

    private void HandleBunkerDepleted()
    {
        SetState(GameState.GameOver);
    }

    private void SetState(GameState nextState)
    {
        if (CurrentState == GameState.GameOver && nextState != GameState.GameOver)
        {
            return;
        }

        if (CurrentState == nextState)
        {
            return;
        }

        CurrentState = nextState;
        ApplyPlayerStateForGameState(nextState);
        OnStateChanged?.Invoke(nextState);
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
