using UnityEngine;
using Mirror;
public class EnemyCore : NetworkBehaviour
{
    public SO_BaseEnemy enemyData;
    public EnemyType enemyType;

    [Header("Components")]
    public EnemyMovement movement;
    public EnemyMeleeAttack attack;
    public EnemyHealth health;

}
