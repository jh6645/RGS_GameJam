using UnityEngine;
using Mirror;
public class ChildTowerHealth : NetworkBehaviour, IEnemyAttackable
{
    private bool _isDead;
    private BaseTower BT;
    public bool isDead { get => _isDead; set => _isDead = value; }
    private void Awake()
    {
        BT = GetComponent<BaseTower>();
    }

    public void TakeDamage(float damage)
    {
        BT.mainTower.towerHealth.MainTakeDamage(damage);
    }
    public void SetDeadState(bool isDead)
    {
        _isDead = isDead;
    }
}
