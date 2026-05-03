using System;
using UnityEngine;

public class EnemyArtilleryTimer : MonoBehaviour
{
    [SerializeField] private GameBalanceConfig gameBalanceConfig;
    [SerializeField] private bool autoStart = true;

    private float baseDurationSeconds = 30f;
    private float maxDurationSeconds = 40f;

    public float CurrentTimeSeconds { get; private set; }
    public bool IsRunning { get; private set; }

    public event Action<float, float> OnTimeChanged;
    public event Action OnTimeout;

    private void Awake()
    {
        if (gameBalanceConfig != null)
        {
            baseDurationSeconds = gameBalanceConfig.baseEnemyTimer;
            maxDurationSeconds = gameBalanceConfig.enemyTimerCap;
        }
    }

    private void Start()
    {
        ResetTimer();

        if (autoStart)
        {
            StartTimer();
        }
    }

    private void Update()
    {
        if (!IsRunning)
        {
            return;
        }

        CurrentTimeSeconds -= Time.deltaTime;
        OnTimeChanged?.Invoke(CurrentTimeSeconds, maxDurationSeconds);

        if (CurrentTimeSeconds <= 0f)
        {
            CurrentTimeSeconds = 0f;
            IsRunning = false;
            OnTimeChanged?.Invoke(CurrentTimeSeconds, maxDurationSeconds);
            OnTimeout?.Invoke();
            ResetTimer();
            StartTimer();
        }
    }

    public void StartTimer()
    {
        IsRunning = true;
        OnTimeChanged?.Invoke(CurrentTimeSeconds, maxDurationSeconds);
    }

    public void StopTimer()
    {
        IsRunning = false;
    }

    public void ResetTimer()
    {
        CurrentTimeSeconds = baseDurationSeconds;
        OnTimeChanged?.Invoke(CurrentTimeSeconds, maxDurationSeconds);
    }

    public void AddTime(float seconds)
    {
        CurrentTimeSeconds = Mathf.Min(CurrentTimeSeconds + Mathf.Max(0f, seconds), maxDurationSeconds);
        OnTimeChanged?.Invoke(CurrentTimeSeconds, maxDurationSeconds);
    }
}
