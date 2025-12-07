using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private SO_BaseTower[] towerDataArray;
    public Dictionary<TowerType, SO_BaseTower> towerDataDict = new Dictionary<TowerType, SO_BaseTower>();

    private void Awake()
    {
        foreach (var data in towerDataArray)
        {
            if (data == null) continue;

            if (!towerDataDict.ContainsKey(data.towerType))
            {
                towerDataDict.Add(data.towerType, data);
            }
            else
                Debug.LogWarning($"중복된 타워 타입: {data.towerType}");
        }
    }

}
