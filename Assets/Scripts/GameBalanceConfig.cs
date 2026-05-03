using UnityEngine;

[CreateAssetMenu(fileName = "GameBalanceConfig", menuName = "ArtTD/Game Balance Config")]
public class GameBalanceConfig : ScriptableObject
{
    [Header("Enemy Artillery")]
    [Min(0f)] public float baseEnemyTimer = 30f;
    [Min(0f)] public float enemyTimerCap = 40f;
    [Min(0f)] public float enemyHitAddsTime = 8f;
    [Min(0)] public int enemyTimeoutDamage = 1;
    [Min(0f)] public float enemyTimerPenalty = 2f;

    [Header("Stun")]
    [Min(0f)] public float stunDuration = 2.5f;

    [Header("Station Sequence Lengths")]
    [Min(1)] public int pickupSequenceMin = 2;
    [Min(1)] public int pickupSequenceMax = 3;
    [Min(1)] public int loadSequenceMin = 3;
    [Min(1)] public int loadSequenceMax = 4;
    [Min(1)] public int unlockSequenceMin = 4;
    [Min(1)] public int unlockSequenceMax = 5;

    [Header("Win Target")]
    [Min(1)] public int hitsToWin = 3;

    [Header("Bunker")]
    [Min(1)] public int maxHp = 3;

    private void OnValidate()
    {
        pickupSequenceMax = Mathf.Max(pickupSequenceMin, pickupSequenceMax);
        loadSequenceMax = Mathf.Max(loadSequenceMin, loadSequenceMax);
        unlockSequenceMax = Mathf.Max(unlockSequenceMin, unlockSequenceMax);
        enemyTimerCap = Mathf.Max(baseEnemyTimer, enemyTimerCap);
    }
}
