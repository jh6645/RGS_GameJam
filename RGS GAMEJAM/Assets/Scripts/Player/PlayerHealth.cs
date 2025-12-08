using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class PlayerHealth : NetworkBehaviour, IEnemyAttackable
{
    [SerializeField] private int playerMaxHealth;
    [SerializeField] private Image playerHealthImg;
    [SerializeField] private Transform dmgIndicatorPos;

    [SyncVar(hook = nameof(OnPlayerHealthChanged))] private int playerHealth;

    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerSetHealth(playerMaxHealth);
    }
    public void TakeDamage(float damage)
    {
        ServerTakeDamage(Mathf.CeilToInt(damage));
    }
    [Server]
    private void ServerTakeDamage(int damage)
    {
        playerHealth -= damage;
        GameManager.Instance.spawnManager.SpawnDmgIndicator(dmgIndicatorPos.position, damage);
    }
    [Server]
    private void ServerSetHealth(int health) 
    {
        playerHealth = health;
    }

    private void OnPlayerHealthChanged(int oldValue, int newValue)
    {
        playerHealthImg.fillAmount = (float)newValue / playerMaxHealth;
    }

}
