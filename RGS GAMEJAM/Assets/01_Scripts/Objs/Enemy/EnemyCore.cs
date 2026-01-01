using UnityEngine;
using Mirror;
using System;


public class EnemyCore : NetworkBehaviour
{
    public SO_BaseEnemy enemyData;
    public EnemyType enemyType;

    [Header("Components")]
    [HideInInspector] public EnemyMovement movement;
    [HideInInspector] public EnemyHealth health;
    [HideInInspector] public Animator animator;
    [HideInInspector] public SpriteRenderer SR;
    [HideInInspector] public BasePooledObject BPO;

    public Transform enemyAttackPos;
    [HideInInspector]public Transform targetPos;
    [HideInInspector] public IEnemyAttackable targetAttackable;

    public EnemyState enemyState;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        health = GetComponent<EnemyHealth>();
        animator = GetComponentInChildren<Animator>();
        SR = GetComponentInChildren<SpriteRenderer>();
        BPO=GetComponent<BasePooledObject>();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        InitServer();
    }
    [Server]
    private void InitServer()
    {
        enemyState = EnemyState.Chasing_Tree;
        health.Init();
        movement.Init();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(enemyAttackPos.position, enemyData.attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(enemyAttackPos.position, enemyData.agroRange_Player);
        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(enemyAttackPos.position, enemyData.agroRange_Tower);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(enemyAttackPos.position, enemyData.agroFinishRange_Player);
    }
}
