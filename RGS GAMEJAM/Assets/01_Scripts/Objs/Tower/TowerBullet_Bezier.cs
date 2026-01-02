using UnityEngine;
using Mirror;

public class TowerBullet_Bezier : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float flightTime = 0.6f;
    [SerializeField] private float arcHeight = 0.6f;

    private Transform target;
    private float damage;

    private Vector2 startPos;
    private Vector2 controlPoint;
    private float elapsed;

    private Rigidbody2D rb;
    private BasePooledObject BPO;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        BPO = GetComponent<BasePooledObject>();
    }

    [Server]
    public void SetBullet(Transform target, float damage)
    {
        this.target = target;
        this.damage = damage;

        startPos = transform.position;

        elapsed = 0f;

        Vector2 initialTargetPos = target.position;

        controlPoint = (startPos + initialTargetPos) * 0.5f + Vector2.up * arcHeight;
    }

    private void FixedUpdate()
    {
        if (!isServer) return;

        if (target == null || !target.gameObject.activeSelf)
        {
            BPO.ServerDespawn();
            return;
        }


        elapsed += Time.fixedDeltaTime;
        float t = elapsed / flightTime;

        if (t >= 1f)
        {
            HitTarget();
            return;
        }

        Vector2 targetPos = target.position;

        // Quadratic Bezier
        Vector2 pos = Mathf.Pow(1 - t, 2) * startPos + 2 * (1 - t) * t * controlPoint + t * t * targetPos;

        rb.MovePosition(pos);
    }

    private void HitTarget()
    {
        if (target != null)
        {
            EnemyCore EC = target.GetComponent<EnemyCore>();
            if (EC != null)
                EC.health.TakeDamage(Mathf.CeilToInt(damage));
        }

        BPO.ServerDespawn();
    }
}
