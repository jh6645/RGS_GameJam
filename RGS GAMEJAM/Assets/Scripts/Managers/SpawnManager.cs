using UnityEngine;
using Mirror;
public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private SO_EnemyDataBase database;

    public override void OnStartServer()
    {
        base.OnStartServer();
        SpawnByType(EnemyType.NormalPerson, new Vector3(20, 20));
    }
    [Server]
    public void SpawnByType(EnemyType type, Vector3 pos)
    {
        GameObject prefab = database.GetPrefab(type);
        GameObject instance = GameManager.Instance.poolManager.Get(prefab);

        instance.transform.position = pos;
        instance.transform.rotation = Quaternion.identity;

        var p = instance.GetComponent<PooledEnemy>();
        p.enemyType = type;
        p.originalPrefab = prefab;

        NetworkServer.Spawn(instance);

        p.OnSpawnFromPool();
    }

}
