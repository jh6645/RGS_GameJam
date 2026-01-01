using UnityEngine;
using Mirror;

public class EnemyMeleeAttack_toggle : NetworkBehaviour
{
    private EnemyCore Core;
    private Transform target;
    private EnemyState prevState;

    [SyncVar(hook = nameof(OnAttackStateChanged))] private bool isAttacking;

    private float attackTimer = 0f;

    private void Awake()
    {
        Core = GetComponent<EnemyCore>();
    }

    private void Update()
    {
        if (!isServer) return;
        if (Core.health.isDead)
        {
            isAttacking = false;
            return;
        }

        attackTimer += Time.deltaTime;

        if (FindNearestTarget())
        {
            isAttacking = true;
            if (Core.enemyState != EnemyState.Attacking)
            {
                prevState = Core.enemyState;
                Core.enemyState = EnemyState.Attacking;
            }
        }
        else
        {
            isAttacking = false;
            if (Core.enemyState == EnemyState.Attacking)
            {
                Core.enemyState = prevState;
            }
        }

        if (attackTimer >= Core.enemyData.attackCool)
        {
            attackTimer = 0f;
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

        RpcPlayAttackEffect();
    }

    [Server]
    private bool FindNearestTarget()
    {
        target = null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            Core.enemyData.attackRange,
            Core.enemyData.towerLayer|Core.enemyData.playerLayer
        );

        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                target = hit.transform;
            }
        }

        return target != null;
    }
    [ClientRpc]
    private void RpcPlayAttackEffect()
    {
        // TODO: 공격 애니메이션
    }

    private void OnAttackStateChanged(bool oldValue, bool newValue)
    {
        Core.animator.SetBool("isAttacking", newValue);
    }
}
