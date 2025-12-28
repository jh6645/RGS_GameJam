using UnityEngine;

public class PooledTower : BasePooledObject
{
    public override void OnSpawnFromPool()
    {
        if (isServer)
        {
            GetComponent<BaseTower>().InitTower();
        }

    }
}
