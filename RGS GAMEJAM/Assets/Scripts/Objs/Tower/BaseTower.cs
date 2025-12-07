using UnityEngine;
using Mirror;
public class BaseTower : NetworkBehaviour
{
    [HideInInspector] public SO_BaseTower towerData;
    public int towerLevel;
    public int currentHP;
    protected virtual void Start()
    {
        currentHP = towerData.towerMaxHP[towerLevel];
    }
}
