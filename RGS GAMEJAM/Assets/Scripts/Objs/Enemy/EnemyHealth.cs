using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class EnemyHealth : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))] public int currentHealth;

    [SerializeField] private Image enemyHealthImg;

    private EnemyCore Core;

    private void Awake()
    {
        Core = GetComponent<EnemyCore>();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth = Core.enemyData.maxHealth;
    }
    [Server]
    public void TakeDamage(int damage)
    {
        RpcHit();
    }
    [ClientRpc]
    private void RpcHit()
    {

    }
    private void OnHealthChanged(int oldValue, int newValue)
    {
        enemyHealthImg.fillAmount = (float)currentHealth / Core.enemyData.maxHealth;
    }

}
