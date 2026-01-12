using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class MainTower : NetworkBehaviour
{
    [HideInInspector] public TowerInteraction towerInteraction;
    [HideInInspector] public MainTowerHealth towerHealth;
    [HideInInspector] public BasePooledObject BPO;

    [Header("Data")]
    public SO_MainTower mainTowerData;

    [SyncVar(hook = nameof(OnTowerLevelChanged))]
    public int towerLevel;

    [SyncVar(hook = nameof(OnPlacementChanged))]
    private Vector2Int originCell;

    [SyncVar(hook = nameof(OnPlacementChanged))]
    private int rotation;

    [SyncVar]
    private int currentHP;

    private readonly List<BaseTower> childTowers = new();

    private void Awake()
    {
        towerHealth = GetComponent<MainTowerHealth>();
        BPO = GetComponent<BasePooledObject>();
        towerInteraction = GetComponent<TowerInteraction>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        CacheChildTowers();
        InitTower();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        CacheChildTowers();
        ApplyPlacement();
    }

    private void CacheChildTowers()
    {
        childTowers.Clear();

        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            BaseTower bt = transform.GetChild(i).GetComponent<BaseTower>();
            if (bt != null) childTowers.Add(bt);
        }
    }

    [Server]
    private void InitTower()
    {
        towerLevel = 0;
        towerHealth.Init();
        towerInteraction.SetTower();

        for (int i = 0; i < childTowers.Count; i++)
        {
            childTowers[i].InitChildTower();
        }
    }


    #region Placement

    [Server]
    public void PlaceMainTower(Vector2Int origin, int rot)
    {
        originCell = origin;
        rotation = rot;

        ApplyPlacement();
    }

    private void ApplyPlacement()
    {
        if (mainTowerData == null) return;
        if (childTowers.Count != mainTowerData.cellPos.Length)
        {
            Debug.LogError("WRONG SETTING");
            return;
        }

        for (int i = 0; i < mainTowerData.cellPos.Length; i++)
        {
            Vector2Int localOffset = mainTowerData.cellPos[i];
            Vector2Int rotatedOffset = GameManager.Instance.gridRenderer.Rotate(localOffset, rotation);
            Vector2Int finalCell = originCell + rotatedOffset;

            childTowers[i].SetTowerPos(finalCell);
        }
    }

    private void OnPlacementChanged(Vector2Int oldValue, Vector2Int newValue)
    {
        ApplyPlacement();
    }

    private void OnPlacementChanged(int oldValue, int newValue)
    {
        ApplyPlacement();
    }

    #endregion

    #region Level / Interaction

    [Server]
    public void AddTowerLevel()
    {
        float hpRatio = (float)towerHealth.currentHP / mainTowerData.towerMaxHP[towerLevel];

        towerLevel++;

        towerHealth.SetCurrentHP(Mathf.CeilToInt(hpRatio * mainTowerData.towerMaxHP[towerLevel]));

        for (int i = 0; i < childTowers.Count; i++)
        {
            childTowers[i].AddChildTowerLevel();
        }
    }

    public void InteractionEnterRange()
    {
        for (int i = 0; i < childTowers.Count; i++)
        {
            childTowers[i].InteractionEnterChildTower();
        }
    }

    public void InteractionExitRange()
    {
        for (int i = 0; i < childTowers.Count; i++)
        {
            childTowers[i].InteractionExitChildTower();
        }
    }

    public void RemoveAllTower()
    {
        for (int i = 0; i < childTowers.Count; i++)
        {
            
            childTowers[i].RemoveTower();

        }
    }

    #endregion

    private void OnTowerLevelChanged(int oldValue, int newValue)
    {
        towerInteraction.SetTower();
    }

}
