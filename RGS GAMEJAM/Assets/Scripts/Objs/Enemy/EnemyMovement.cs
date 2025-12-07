using UnityEngine;
using Mirror;
public class EnemyMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float agroRange = 5f;

    private Rigidbody2D rb;
    private Transform mainTarget;
    private Transform currentTarget;

    [SerializeField] private LayerMask towerLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool agroPlayer;
    [SerializeField] private bool agroTower;

    private void Awake()
    {
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

        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAgroTarget()
    {
        Transform nearest = null;
        float nearestDist = Mathf.Infinity;

        if (agroTower)
        {
            Collider2D[] col_tower = Physics2D.OverlapCircleAll(transform.position, agroRange, towerLayer);

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

        if (agroPlayer)
        {
            Collider2D[] col_player = Physics2D.OverlapCircleAll(transform.position, agroRange, playerLayer);

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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, agroRange);
    }
}
