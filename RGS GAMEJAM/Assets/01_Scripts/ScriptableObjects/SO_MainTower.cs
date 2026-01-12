using System;
using UnityEngine;
[CreateAssetMenu(fileName = "SO_MainTower", menuName = "Scriptable Objects/SO_MainTower")]
public class SO_MainTower : ScriptableObject
{
    public MainTowerType towerType;
    public Vector2Int[] cellPos;
    public int[] towerMaxHP = new int[5];
    public TowerStuff[] upgradeStuffs = new TowerStuff[5];
    public float placeTime;
    public int craftingAmount = 1;

    public string towerName;
    [TextArea]
    public string towerInfo;
    public Sprite towerIcon;
}
