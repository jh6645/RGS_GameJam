using Mirror;
using UnityEngine;

public class EnemyMeleeAttack : NetworkBehaviour
{
    private EnemyCore Core;
    private float lastAttackTime;
    private Transform target;
    private void Awake()
    {
        Core = GetComponent<EnemyCore>();
    }

    private void Update()
    {
        if (!isServer) return;

        target = Core.movement.currentTarget;
        if (target == null) return;

        // 애니메이션 오면 수정해야함
        float distance = Vector2.Distance(transform.position, target.position);
        if (distance <= Core.enemyData.attackRange && Time.time >= lastAttackTime + Core.enemyData.attackCool)
        {
            PerformAttack();
        }
    }

    [Server]
    private void PerformAttack()
    {
        if (target == null) return;

        var attackable = target.GetComponent<IEnemyAttackable>();
        if (attackable != null)
        {
            attackable.TakeDamage(Core.enemyData.attackDamage);
        }

        lastAttackTime = Time.time;
        RpcPlayAttackEffect();
    }

    [ClientRpc]
    private void RpcPlayAttackEffect()
    {
        Debug.Log("Enemy attacked (2D)!");
        //TODO: 애니메이션 추가하기
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Core.enemyData.attackRange);
    }
}
