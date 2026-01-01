using UnityEngine;

public interface IEnemyAttackable
{
    public bool isDead { get; set; }
    public void TakeDamage(float damage);
}