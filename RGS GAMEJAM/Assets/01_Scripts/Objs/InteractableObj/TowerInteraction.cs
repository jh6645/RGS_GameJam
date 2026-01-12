using UnityEngine;
using Mirror;
using Unity.Services.Lobbies.Models;
using System.Resources;
public class TowerInteraction : NetworkBehaviour, IInteractable
{
    [Header("UI Prompt")]
    [SerializeField] private Transform appearTransform;
    [SerializeField] private bool useAppearTransform = true;
    public GameObject Obj => gameObject;
    [SyncVar] public bool isLocked;
    public bool IsLocked { get => isLocked; set => isLocked = value; }
    [SyncVar] private NetworkIdentity lockedBy;
    public NetworkIdentity LockedBy { get => lockedBy; set => lockedBy = value; }
    public Transform AppearTransform => useAppearTransform && appearTransform != null ? appearTransform : transform;
    public bool isAppearTransform => useAppearTransform && appearTransform != null;
    public bool isRoomInteractor => false;
    public InteractionType GetInteractionType() => InteractionType.Hold;
    public float GetHoldTime() => MT.mainTowerData.placeTime;
    public string GetPromptText() => "타워 업그레이드";

    private MainTower MT;
    [SerializeField] private TowerInteractionUI towerUI;
    [SerializeField] private GameObject upgradePanel;
    private void Awake()
    {
        MT = GetComponentInParent<MainTower>();
    }
    public bool CanInteract(Interactor interactor)
    {
        if (IsLocked && lockedBy != interactor.netIdentity) return false;
        if (MT.towerLevel >= 4) return false;
        if (CanUpgradeResources())
        {
            return true;
        }
        return false;
    }

    public bool InteractClient(Interactor interactor)
    {
        return true;
    }

    public void OnEnterRange(Interactor interactor)
    {
        towerUI.SetTower(MT);
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
        }
        MT.InteractionEnterRange();
    }

    public void OnExitRange(Interactor interactor)
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
        MT.InteractionExitRange();
    }

    [Server]
    public bool InteractServer(Interactor interactor)
    {
        if (!CanUpgradeResources())
        {
            return false;
        }
        ResourceManager resourceManager = GameManager.Instance.resourceManager;
        resourceManager.ServerRemoveLeaf(MT.mainTowerData.upgradeStuffs[MT.towerLevel+1].needLeaf);
        resourceManager.ServerRemoveStick(MT.mainTowerData.upgradeStuffs[MT.towerLevel+1].needStick);
        resourceManager.ServerRemoveStone(MT.mainTowerData.upgradeStuffs[MT.towerLevel+1].needStone);

        MT.AddTowerLevel();
        return true;
    }
    public void SetTower()
    {
        towerUI.SetTower(MT);
    }
    private bool CanUpgradeResources()
    {
        ResourceManager resourceManager = GameManager.Instance.resourceManager;
        return resourceManager.leaf >= MT.mainTowerData.upgradeStuffs[MT.towerLevel+1].needLeaf &&
            resourceManager.stick >= MT.mainTowerData.upgradeStuffs[MT.towerLevel + 1].needStick &&
            resourceManager.stone >= MT.mainTowerData.upgradeStuffs[MT.towerLevel + 1].needStone;
    }
}
