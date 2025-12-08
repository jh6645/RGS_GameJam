using UnityEngine;
using Mirror;
public class EnemyMovement : NetworkBehaviour
{
    private Rigidbody2D rb;
    private Transform mainTarget;
    [HideInInspector] public Transform currentTarget;
    private EnemyCore Core;

    private void Awake()
    {
        Core = GetComponent<EnemyCore>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        mainTarget = GameObject.FindGameObjectWithTag("mainTree").transform;
    }

    private void FixedUpdate()
    {
        if (!isServer) return;

        UpdateAgroTarget();

        Vector2 dir;

        if (currentTarget != null)
        {
            dir = (currentTarget.position - transform.position).normalized;
        }
        else
        {
            dir = (mainTarget.position - transform.position).normalized; 
        }

        rb.MovePosition(rb.position + dir * Core.enemyData.moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAgroTarget()
    {
        Transform nearest = null;
        float nearestDist = Mathf.Infinity;

        if (Core.enemyData.isAgroTower)
        {
            Collider2D[] col_tower = Physics2D.OverlapCircleAll(transform.position, Core.enemyData.agroRange, Core.enemyData.towerLayer);

            foreach (var col in col_tower)
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = col.transform;
                }
            }
        }

        if (Core.enemyData.isAgroPlayer)
        {
            Collider2D[] col_player = Physics2D.OverlapCircleAll(transform.position, Core.enemyData.agroRange, Core.enemyData.playerLayer);

            foreach (var col in col_player)
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = col.transform;
                }
            }
        }

        currentTarget = nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Core.enemyData.agroRange);
    }
}
