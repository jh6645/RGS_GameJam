using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class PlayerTower : NetworkBehaviour
{
    [SerializeField] private Image towerImg;
    [SyncVar(hook =nameof(OnTowerChanged))] public bool isMovingTower = false;
    [SyncVar] private TowerType nowTowerType;
    private Vector2 selectedCell;
    
    private void Start()
    {
        isTowerImgShowing(false);
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        if (isMovingTower)
        {
            var grid = GameManager.Instance.gridRenderer;
            selectedCell = grid.HighlightCell(new Vector2(transform.position.x, transform.position.y-0.35f));
        }

    }

    private void isTowerImgShowing(bool isShowing)
    {
        Color color = towerImg.color;
        color.a = isShowing ? 1 : 0;
        towerImg.color = color;
    }
    public void AddTower(SO_BaseTower BT)
    {
        if (isMovingTower) return;
        Debug.Log(BT.towerType.ToString());
        GameManager.Instance.gridRenderer.ToggleGrid(true);
        CmdAddTower(BT.towerType);
    }
    [Command]
    private void CmdAddTower(TowerType towerType) 
    { 
        ServerAddTower(towerType);
    }
    [Server]
    private void ServerAddTower(TowerType towerType)
    {
        nowTowerType = towerType;
        isMovingTower = true;
    }
    private void OnTowerChanged(bool oldValue, bool newValue)
    {
        towerImg.sprite = GameManager.Instance.towerManager.towerDataDict[nowTowerType].towerIcon;
        isTowerImgShowing(newValue);
    }
}
