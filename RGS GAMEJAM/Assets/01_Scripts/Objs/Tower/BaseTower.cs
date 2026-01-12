using UnityEngine;
using Mirror;
using UnityEngine.UI;
public class BaseTower : NetworkBehaviour
{
    public SO_BaseTower towerData;
    public MainTower mainTower;

    [HideInInspector] public TowerAttack towerAttack;
    [HideInInspector] public Animator animator;
    [HideInInspector] public ChildTowerHealth health;
    private Vector2Int towerPos;
    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        towerAttack = GetComponent<TowerAttack>();
        health = GetComponent<ChildTowerHealth>();
    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {
    }

    [Server]
    public void AddChildTowerLevel()
    {
        animator.SetInteger("Level", mainTower.towerLevel);
        animator.SetTrigger("LevelUp");
    }
    [Server]
    public void InitChildTower()
    {
        health.SetDeadState(false);
        animator.SetInteger("Level", mainTower.towerLevel);
    }
    public void InteractionEnterChildTower()
    {
        if (towerAttack != null)
        {
            towerAttack.SetRange(true);
        }
    }
    public void InteractionExitChildTower()
    {
        if (towerAttack != null)
        {
            towerAttack.SetRange(false);
        }
    }
    public void SetTowerPos(Vector2Int pos)
    {
        towerPos = pos;
        Vector2 cartesiancoord = GameManager.Instance.gridRenderer.ToCartesian2D(pos.x + 0.5f, pos.y + 0.5f);
        gameObject.transform.position = new Vector2(cartesiancoord.x, cartesiancoord.y - 0.5f);
        if (isServer)
        {
            GameManager.Instance.towerManager.Place(pos.x + 24, pos.y + 24, this, false);
        }
    }
   
    public void RemoveTower()
    {
        health.SetDeadState(true);
        GameManager.Instance.towerManager.Remove(towerPos.x+24, towerPos.y+24);
    }
}
