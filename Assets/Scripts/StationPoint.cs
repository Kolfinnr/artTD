using UnityEngine;

public enum StationType
{
    ShellRack,
    BreechLoader,
    GunUnlockConsole
}

[RequireComponent(typeof(Collider2D))]
public class StationPoint : InteractableStation
{
    [SerializeField] private StationType stationType;
    [SerializeField] private GameLoopManager gameLoopManager;

    private bool completed;

    public override void Interact(PlayerController player)
    {
        if (completed)
        {
            return;
        }

        if (gameLoopManager == null)
        {
            gameLoopManager = FindFirstObjectByType<GameLoopManager>();
        }

        completed = true;
        gameLoopManager?.ReportPrepTaskCompleted();
    }

    public void ResetStation()
    {
        completed = false;
    }
}
