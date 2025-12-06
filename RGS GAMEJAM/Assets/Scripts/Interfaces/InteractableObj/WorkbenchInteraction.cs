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

    [Header("Tower Data")]
    [SerializeField] private SO_BaseTower[] towerDataArray;

    private Dictionary<TowerType, bool> isRegisteredTowerDict;
    private Dictionary<TowerType, SO_BaseTower> towerDataDict;

    private void Awake()
    {
        isRegisteredTowerDict = new Dictionary<TowerType, bool>();
        towerDataDict = new Dictionary<TowerType, SO_BaseTower>();

        foreach (TowerType type in Enum.GetValues(typeof(TowerType)))
            isRegisteredTowerDict[type] = false;

        foreach (var data in towerDataArray)
        {
            if (data == null) continue;

            if (!towerDataDict.ContainsKey(data.towerType))
                towerDataDict.Add(data.towerType, data);
            else
                Debug.LogWarning($"중복된 타워 타입: {data.towerType}");
        }
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

            if (!towerDataDict.ContainsKey(towerInfo.Key))
            {
                Debug.LogError($"NO {towerInfo.Key}");
                continue;
            }

            // UI 생성
            GameObject go = Instantiate(towerUI, content);
            go.GetComponent<TowerInfoUI>().SetTowerInfo(towerDataDict[towerInfo.Key]);

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
