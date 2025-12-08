using UnityEngine;
using Mirror;
public class SpawnManager : NetworkBehaviour
{
    public GameObject[] enemies;
    public GameObject dmgIndicator;
    [SerializeField] private PoolManager poolManager;
    private void Awake()
    {
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        poolManager.Spawn<PooledEnemy>(enemies[0], new Vector2(15, 15));
    }
    #region SpawnDmgIndicator
    [Server]
    public void SpawnDmgIndicator(Vector2 position, int damage)
    {
        PooledDamageIndicator PDI = poolManager.Spawn<PooledDamageIndicator>(dmgIndicator, position);
        PDI.SetIndicator(damage);
    }
    #endregion


}
