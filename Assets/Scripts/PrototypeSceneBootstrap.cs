using UnityEngine;
using UnityEngine.UI;

public class PrototypeSceneBootstrap : MonoBehaviour
{
    [SerializeField] private GameBalanceConfig gameBalanceConfig;

    private void Start()
    {
        BuildWorld();
        BuildUI();
    }

    private void BuildWorld()
    {
        GameObject systems = new GameObject("Systems");
        EnemyArtilleryTimer timer = systems.AddComponent<EnemyArtilleryTimer>();
        BunkerHealth health = systems.AddComponent<BunkerHealth>();
        GameLoopManager loop = systems.AddComponent<GameLoopManager>();

        AttachConfig(timer, health, loop);

        CreateCorridor();
        CreateLadder();
        CreateStations(loop);
        CreateGunPosition();
        CreateEnemyTarget();
        CreatePlayer();
    }

    private void AttachConfig(EnemyArtilleryTimer timer, BunkerHealth health, GameLoopManager loop)
    {
        // manual inspector assignment preferred; this bootstrap keeps runtime setup minimal.
        if (gameBalanceConfig == null)
        {
            return;
        }

        typeof(EnemyArtilleryTimer).GetField("gameBalanceConfig", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(timer, gameBalanceConfig);
        typeof(BunkerHealth).GetField("gameBalanceConfig", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(health, gameBalanceConfig);
        typeof(GameLoopManager).GetField("gameBalanceConfig", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(loop, gameBalanceConfig);
    }

    private void CreateCorridor()
    {
        GameObject floor = new GameObject("Corridor");
        floor.transform.position = new Vector3(0f, -2f, 0f);
        BoxCollider2D floorCollider = floor.AddComponent<BoxCollider2D>();
        floorCollider.size = new Vector2(24f, 1f);

        CreateWall("LeftWall", new Vector2(-12f, 0f), new Vector2(1f, 8f));
        CreateWall("RightWall", new Vector2(12f, 0f), new Vector2(1f, 8f));
    }

    private void CreateWall(string name, Vector2 position, Vector2 size)
    {
        GameObject wall = new GameObject(name);
        wall.transform.position = position;
        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.size = size;
    }

    private void CreateLadder()
    {
        GameObject ladder = new GameObject("LadderZone");
        ladder.transform.position = new Vector3(-5f, 0f, 0f);
        ladder.AddComponent<LadderZone>();
        BoxCollider2D box = ladder.GetComponent<BoxCollider2D>();
        box.size = new Vector2(1.2f, 6f);
        box.isTrigger = true;
    }

    private void CreateStations(GameLoopManager loop)
    {
        CreateStation("Shell Rack", new Vector2(-8f, -1f), StationType.ShellRack, loop);
        CreateStation("Breech Loader", new Vector2(0f, -1f), StationType.BreechLoader, loop);
        CreateStation("Gun Unlock Console", new Vector2(7f, -1f), StationType.GunUnlockConsole, loop);
    }

    private void CreateStation(string name, Vector2 pos, StationType type, GameLoopManager loop)
    {
        GameObject station = new GameObject(name);
        station.transform.position = pos;
        CircleCollider2D trigger = station.AddComponent<CircleCollider2D>();
        trigger.radius = 0.8f;
        trigger.isTrigger = true;

        StationPoint stationPoint = station.AddComponent<StationPoint>();
        typeof(StationPoint).GetField("stationType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(stationPoint, type);
        typeof(StationPoint).GetField("gameLoopManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(stationPoint, loop);
    }

    private void CreateGunPosition()
    {
        GameObject gun = new GameObject("Gun Aim Position");
        gun.transform.position = new Vector3(10f, -1f, 0f);
    }

    private void CreateEnemyTarget()
    {
        GameObject enemy = new GameObject("Enemy Bunker Target");
        enemy.transform.position = new Vector3(22f, 2f, 0f);
        BoxCollider2D box = enemy.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size = new Vector2(2f, 2f);
        enemy.AddComponent<EnemyTarget>();
    }

    private void CreatePlayer()
    {
        GameObject player = new GameObject("Player");
        player.transform.position = new Vector3(-10f, -1f, 0f);
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        player.AddComponent<CircleCollider2D>();
        player.AddComponent<PlayerStateController>();
        player.AddComponent<PlayerController>();

        GameObject trigger = new GameObject("InteractionTrigger");
        trigger.transform.SetParent(player.transform, false);
        CircleCollider2D triggerCollider = trigger.AddComponent<CircleCollider2D>();
        triggerCollider.radius = 1.4f;
        triggerCollider.isTrigger = true;
    }

    private void BuildUI()
    {
        GameObject canvasObject = new GameObject("HUD Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        CreateLabel(canvas.transform, "HP: 3", new Vector2(80f, -30f));
        CreateLabel(canvas.transform, "Enemy Timer: 30", new Vector2(150f, -60f));
        CreateLabel(canvas.transform, "Tasks: Shell / Load / Unlock", new Vector2(190f, -100f));
        CreateLabel(canvas.transform, "Press E to interact", new Vector2(0f, 70f), TextAnchor.LowerCenter);
        CreateLabel(canvas.transform, "Sequence: _ _ _", new Vector2(0f, 40f), TextAnchor.LowerCenter);

        GameObject overlay = new GameObject("Overlay");
        overlay.transform.SetParent(canvas.transform, false);
        Image overlayImage = overlay.AddComponent<Image>();
        overlayImage.color = new Color(1f, 0f, 0f, 0f);
        RectTransform rt = overlay.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private void CreateLabel(Transform parent, string textValue, Vector2 anchoredPos, TextAnchor anchor = TextAnchor.UpperLeft)
    {
        GameObject label = new GameObject(textValue);
        label.transform.SetParent(parent, false);
        Text text = label.AddComponent<Text>();
        text.text = textValue;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = anchor;

        RectTransform rect = label.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400f, 40f);

        if (anchor == TextAnchor.UpperLeft)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
        }
        else
        {
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
        }

        rect.anchoredPosition = anchoredPos;
    }
}
