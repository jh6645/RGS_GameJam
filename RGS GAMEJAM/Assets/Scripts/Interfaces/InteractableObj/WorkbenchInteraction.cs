using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkbenchInteraction : MonoBehaviour, IInteractable
{
    [Header("UI Prompt")]
    [SerializeField] private Transform appearTransform;
    [SerializeField] private bool useAppearTransform = true;
    public Transform AppearTransform => useAppearTransform && appearTransform != null ? appearTransform : transform;
    public bool isAppearTransform => useAppearTransform && appearTransform != null;
    public bool isRoomInteractor => false;
    public InteractionType GetInteractionType() => InteractionType.Tap;
    public float GetHoldTime() => 0f;
    public string GetPromptText() => "제작대 열기";

    [Header("Workbench UI")]
    [SerializeField] private GameObject workbenchPanel;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject towerUI;

    private Dictionary<TowerType, bool> isRegisteredTowerDict;
   

    private void Awake()
    {
        isRegisteredTowerDict = new Dictionary<TowerType, bool>();

        foreach (TowerType type in Enum.GetValues(typeof(TowerType)))
            isRegisteredTowerDict[type] = false;


    }

    public bool CanInteract() => true;

    public bool Interact(Interactor interactor)
    {
        return true;
    }

    public void OnEnterRange(Interactor interactor)
    {
        if (workbenchPanel == null) return;

        foreach (var towerInfo in GameManager.Instance.resourceManager.bluePrints)
        {
            if (!towerInfo.Value) continue;
            if (isRegisteredTowerDict[towerInfo.Key]) continue;

            if (!GameManager.Instance.towerManager.towerDataDict.ContainsKey(towerInfo.Key))
            {
                Debug.LogError($"NO {towerInfo.Key}");
                continue;
            }

            GameObject go = Instantiate(towerUI, content);
            go.GetComponent<TowerInfoUI>().SetTowerInfo(GameManager.Instance.towerManager.towerDataDict[towerInfo.Key], () => {
                CustomNetworkGamePlayer.localPlayer.playerTower.AddTower(GameManager.Instance.towerManager.towerDataDict[towerInfo.Key]);
            });

            isRegisteredTowerDict[towerInfo.Key] = true;
        }

        workbenchPanel.SetActive(true);
    }

    public void OnExitRange(Interactor interactor)
    {
        if (workbenchPanel != null)
            workbenchPanel.SetActive(false);
    }
}
