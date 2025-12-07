using UnityEngine;
using Mirror;
public class PooledEnemy : NetworkBehaviour
{
    [HideInInspector] public EnemyType enemyType;
    [HideInInspector] public GameObject originalPrefab;

    public void OnSpawnFromPool()
    {
        gameObject.SetActive(true);
        // 여기서 초기화 ㄱㄱ
    }

    [Server]
    public void ServerDespawn()
    {
        NetworkServer.UnSpawn(gameObject);
        RpcReturnToPool();
    }

    [ClientRpc]
    void RpcReturnToPool()
    {
        GameManager.Instance.poolManager.Return(originalPrefab, gameObject);
    }
}
