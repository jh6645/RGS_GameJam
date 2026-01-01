using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
public class EnemyMovement : NetworkBehaviour
{
    private Rigidbody2D rb;
    private Transform mainTarget;
    private EnemyCore Core;

    private Vector2Int nowCell;
    private List<Vector2Int> path;
    private int pathIndex = 0;
    private float pathUpdateInterval = 0.5f;
    private Coroutine pathUpdateCoroutine;
    [SyncVar(hook = nameof(OnFlipChanged))] private bool flipX;
    private void Awake()
    {
        Core = GetComponent<EnemyCore>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {

    }
    private void Update()
    {
        if (!isServer) return;
        nowCell = GameManager.Instance.gridRenderer.CalcHighlightCell_Array(transform.position);
    }
    private void FixedUpdate()
    {
        if (!isServer) return;
        if (Core.health.isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        if (Core.enemyState == EnemyState.Chasing_Player || Core.enemyState == EnemyState.Chasing_Target)
        {
            if (Core.targetAttackable.isDead)
            {
                Core.enemyState = EnemyState.Chasing_Tree;
                Core.targetPos = null;
                Core.targetAttackable = null;
            }

        }

        if (Core.enemyState == EnemyState.Chasing_Tree)
        {
            UpdateAgroTarget();
        }
        if (Core.enemyState == EnemyState.Chasing_Player)
        {
            CheckPlayerDistance();
        }
        if (Core.enemyState == EnemyState.Chasing_Tree || Core.enemyState == EnemyState.Chasing_Target || Core.enemyState == EnemyState.Chasing_Player)
        {
            Move();
        }
        if (Core.enemyState == EnemyState.Attacking)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    public void Init()
    {
        mainTarget = GameObject.FindGameObjectWithTag("mainTree").transform;
        flipX = false;
        nowCell = GameManager.Instance.gridRenderer.CalcHighlightCell_Array(transform.position);
        StartPathUpdating();
    }
    private void Move()
    {

        if (path == null || pathIndex >= path.Count)
        {
            Vector2 dir = ((Vector2)mainTarget.position - rb.position).normalized;
            rb.MovePosition(rb.position + dir * Core.enemyData.moveSpeed * Time.fixedDeltaTime);
            FlipSprite(dir.x);
            return;
        }

        Vector3 targetPos = GameManager.Instance.gridRenderer.ToCartesian2D(path[pathIndex].x - 24, path[pathIndex].y - 24);
        targetPos.y -= 0.25f;
        Vector2 moveDir = (targetPos - transform.position).normalized;

        float distanceThisFrame = Core.enemyData.moveSpeed * Time.fixedDeltaTime;
        if (Vector2.Distance(transform.position, targetPos) <= distanceThisFrame)
        {
            rb.MovePosition(targetPos);
            pathIndex++;
        }
        else
        {
            rb.MovePosition(rb.position + moveDir * distanceThisFrame);
        }

        if (Mathf.Abs(moveDir.x) >= 0.02f)
        {
            FlipSprite(moveDir.x);
        }
    }
    private void FlipSprite(float dirX)
    {
        flipX = dirX < 0;
    }

    public void StartPathUpdating()
    {
        if (pathUpdateCoroutine != null) StopCoroutine(pathUpdateCoroutine);
        pathUpdateCoroutine = StartCoroutine(PathUpdateRoutine());
    }

    public void StopPathUpdating()
    {
        if (pathUpdateCoroutine != null) StopCoroutine(pathUpdateCoroutine);
    }

    private IEnumerator PathUpdateRoutine()
    {
        while (true)
        {
            SetPath();
            yield return new WaitForSeconds(pathUpdateInterval);
        }
    }
    private void UpdateAgroTarget()
    {

        Transform nearest = null;
        float nearestDist = Mathf.Infinity;

        if (Core.enemyData.isAgroPlayer)
        {
            Collider2D[] col_player = Physics2D.OverlapCircleAll(Core.enemyAttackPos.position, Core.enemyData.agroRange_Player, Core.enemyData.playerLayer);

            foreach (var col in col_player)
            {
                float dist = Vector2.Distance(Core.enemyAttackPos.position, col.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = col.transform;
                    Core.enemyState = EnemyState.Chasing_Player;
                }
            }
        }
        if (nearest != null)
        {
            Core.targetPos = nearest;
            Core.targetAttackable = nearest.GetComponent<IEnemyAttackable>();
            return;
        }

        if (Core.enemyData.isAgroTower)
        {
            Collider2D[] col_tower = Physics2D.OverlapCircleAll(Core.enemyAttackPos.position, Core.enemyData.agroRange_Tower, Core.enemyData.towerLayer);

            foreach (var col in col_tower)
            {
                float dist = Vector2.Distance(Core.enemyAttackPos.position, col.transform.position);
                if (dist < nearestDist)
                {
                    BaseTower BT = col.GetComponent<BaseTower>();
                    if (!BT.towerData.isWall)
                    {
                        nearestDist = dist;
                        nearest = col.transform;
                        Core.enemyState = EnemyState.Chasing_Target;
                    }
                }
            }
        }



        Core.targetPos = nearest;
        if (nearest != null)
        {
            Core.targetAttackable = nearest.GetComponent<IEnemyAttackable>();
        }
    }

    private void CheckPlayerDistance()
    {
        if (Core.targetPos == null)
        {
            Core.enemyState = EnemyState.Chasing_Tree;
            Core.targetAttackable = null;
            return;
        }

        float dist = Vector2.Distance(transform.position, Core.targetPos.position);

        if (dist > Core.enemyData.agroFinishRange_Player)
        {
            Core.enemyState = EnemyState.Chasing_Tree;
            Core.targetAttackable = null;
            Core.targetPos = null;
        }
    }
    private void OnFlipChanged(bool oldValue, bool newValue)
    {
        Core.SR.flipX = newValue;
    }
    public void SetPath()
    {
        List<Vector2Int> newpath = null;
        if (Core.enemyState == EnemyState.Chasing_Tree)
        {
            newpath = GameManager.Instance.pathFindingManager.FindPath(nowCell, new Vector2Int(22, 21));
        }
        else if (Core.enemyState == EnemyState.Chasing_Player|| Core.enemyState == EnemyState.Chasing_Target)
        {
            Vector2Int targetPosIsometric = GameManager.Instance.gridRenderer.CalcHighlightCell_Array(Core.targetPos.position);
            newpath = GameManager.Instance.pathFindingManager.FindPath(nowCell, targetPosIsometric);
        }
        else
        {
            return;
        }
        if (newpath != null) { 
            GameManager.Instance.pathFindingManager.SetDebugPath(newpath);
        }
        path = newpath;
        pathIndex = 0;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isServer) return;
        if (collision.collider.CompareTag("mainTree"))
        {
            GameManager.Instance.mainTree.OnTreeAttacked(Core.health.currentHealth);
            Core.BPO.ServerDespawn();
        }
    }
}
