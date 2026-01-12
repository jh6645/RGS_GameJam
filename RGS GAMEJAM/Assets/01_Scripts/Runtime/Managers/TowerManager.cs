using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class TowerManager : NetworkBehaviour
{
    [SerializeField] private SO_MainTower[] towerDataArray;
    public Dictionary<MainTowerType, SO_MainTower> towerDataDict = new Dictionary<MainTowerType, SO_MainTower>();

    private bool[,] occupied = new bool[48, 48];
    private BaseTower[,] towerDatas = new BaseTower[48, 48];

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
    public override void OnStartServer()
    {
        base.OnStartServer();
        //workbench
        Place(15, 23, null, false);
        Place(15, 24, null, false);
        Place(16, 23, null, false);
        Place(16, 24, null, false);
        //tree
        Place(21, 21, null, true);
        Place(21, 22, null, true);
        Place(22, 21, null, true);
        Place(22, 22, null, true);
        //chest
        Place(26, 19, null, false);
        //Composter
        Place(29, 32, null, false);
    }
    [Server]
    public bool CanPlace(Vector2Int origin, Vector2Int[] shape, int rotation)
    {
        
        foreach (var cell in shape)
        {
            Vector2Int rotated = GameManager.Instance.gridRenderer.Rotate(cell, rotation);
            Vector2Int pos = origin + rotated;
            pos.x += 24;
            pos.y += 24;
            if (pos.x < 0 || pos.y < 0 || pos.x >= 48 || pos.y >= 48)
                return false;

            if (occupied[pos.x, pos.y])
                return false;
        }
        return true;
    }

    [Server]
    public void Place(int x, int y, BaseTower BT, bool isTree)
    {
        occupied[x, y] = true;
        if (BT != null)
        {
            towerDatas[x, y] = BT;
            GameManager.Instance.pathFindingManager.AddTowerNode(x, y, BT.towerData.towerWeight);
        }
        else
        {
            if (isTree)
            {
                GameManager.Instance.pathFindingManager.AddTowerNode(x, y, 1);
            }
            else
            {
                GameManager.Instance.pathFindingManager.AddWall(x, y);
            }
        }
    }

    [Server]
    public void Remove(int x, int y)
    {
        occupied[x, y] = false;
        GameManager.Instance.pathFindingManager.RemoveNode(x, y);
    }
    [Server] 
    public void PlaceTower(int x, int y, MainTowerType type, int rotation)
    {
        Vector2 cartesiancoord = GameManager.Instance.gridRenderer.ToCartesian2D(x+0.5f, y+0.5f);
        GameObject towerObj = GameManager.Instance.spawnManager.SpawnTower(type, new Vector2(cartesiancoord.x, cartesiancoord.y - 0.5f));
        towerObj.GetComponent<MainTower>().PlaceMainTower(new Vector2Int(x, y), rotation);
        //Place(x + 24, y + 24, towerObj.GetComponent<BaseTower>(),false);
    }
}
