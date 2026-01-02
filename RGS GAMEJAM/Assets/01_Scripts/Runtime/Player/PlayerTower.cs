using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
public class PlayerTower : NetworkBehaviour
{
    [SerializeField] private Image towerImg;
    [SerializeField] private TMP_Text towerAmountTxt;
    [SyncVar(hook =nameof(OnTowerAmountChanged))] public int holdingTowerAmount = 0;
    [SyncVar(hook = nameof(OnTowerChanged))] public TowerType nowTowerType;
    [SyncVar] public bool isCellEmpty = false;

    private Vector2Int occupiedCell;
    private Vector2Int selectedCell;
    private Vector2Int lastSentCell = new Vector2Int(0, 0);

    [SerializeField] private GameObject playerFollowingPrefab;
    private GameObject towerInteractionTrigger;
    private PlayerInputHandler playerInputHandler;

    public bool isCellOutOfRange;
    public bool isMovingTower => holdingTowerAmount > 0;
    private void Awake()
    {
        playerInputHandler = GetComponent<PlayerInputHandler>();
    }
    private void Start()
    {
        isTowerImgShowing(false);
    }
    private void Update()
    {
        if (!isLocalPlayer) return;

        if (isMovingTower)
        {
            occupiedCell = GameManager.Instance.gridRenderer.CalcHighlightCell(new Vector2(transform.position.x, transform.position.y - 0.35f));

            selectedCell = GameManager.Instance.gridRenderer.HighlightCell(playerInputHandler.GetMouseWorldPosition2D(), occupiedCell, out isCellOutOfRange);

            if (selectedCell != lastSentCell)
            {
                lastSentCell = selectedCell;
                CmdSetSelectedCell(selectedCell);
            }
        }
        else
        {
            GameManager.Instance.gridRenderer.UnHighlightCell();
        }
    }
    public void SetTowerTrigger(GameObject trigger)
    {
        towerInteractionTrigger = trigger;
        towerInteractionTrigger.SetActive(false);
        towerInteractionTrigger.GetComponent<TowerPlaceInteraction>().SetPlayerTower(this);
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
        if (!isEnoughResources(BT)) return;
        CmdUseResources(BT.upgradeStuffs[0].needLeaf, BT.upgradeStuffs[0].needStick, BT.upgradeStuffs[0].needStone);
        towerInteractionTrigger.SetActive(true);
        GameManager.Instance.gridRenderer.ToggleGrid(true);
        towerInteractionTrigger.GetComponent<TowerPlaceInteraction>().SetTowerData(BT);
        CmdAddTower(BT.towerType, BT.craftingAmount);
    }
    private bool isEnoughResources(SO_BaseTower BT)
    {
        ResourceManager RM = GameManager.Instance.resourceManager;
        return BT.upgradeStuffs[0].needLeaf <= RM.leaf &&
            BT.upgradeStuffs[0].needStick <= RM.stick &&
            BT.upgradeStuffs[0].needStone <= RM.stone;
    }
    public Vector2Int GetSelectedPos()
    {
        return selectedCell;
    }
    [Command]
    private void CmdUseResources(int needLeaf, int needStick, int needStone)
    {
        ResourceManager RM = GameManager.Instance.resourceManager;
        RM.ServerRemoveLeaf(needLeaf);
        RM.ServerRemoveStick(needStick);
        RM.ServerRemoveStone(needStone);
    }

    [Command]
    private void CmdAddTower(TowerType towerType, int towerAmount) 
    {
        nowTowerType = towerType;
        holdingTowerAmount = towerAmount;
    }
    public void RemoveTower()
    {
        if (holdingTowerAmount == 1)
        {
            towerInteractionTrigger.SetActive(false);
            GameManager.Instance.gridRenderer.ToggleGrid(false);
            GameManager.Instance.gridRenderer.UnHighlightCell();
        }

        CmdRemoveTower();

    }
    [Command]
    private void CmdRemoveTower()
    {
        Vector2Int towerPos = selectedCell;
        GameManager.Instance.towerManager.PlaceTower(towerPos.x, towerPos.y, nowTowerType);
        holdingTowerAmount -= 1;
        if (!isMovingTower)
        {
            nowTowerType = TowerType.NONE;
        }
    }


    private void OnTowerChanged(TowerType oldValue, TowerType newValue)
    {
        if (newValue!=TowerType.NONE)
        {
            towerImg.sprite = GameManager.Instance.towerManager.towerDataDict[newValue].towerIcon;
        }
        else
        {
            towerImg.sprite = null;
        }
        isTowerImgShowing(newValue!=TowerType.NONE);
    }
    private void OnTowerAmountChanged(int oldValue, int newValue)
    {
        if (newValue != 0)
        {
            towerAmountTxt.text = holdingTowerAmount.ToString();
        }
        else
        {
            towerAmountTxt.text = "";
        }
        
    }
    public bool CheckCellEmpty()
    {
        CmdCheckCell(selectedCell);
        return isCellEmpty;
    }
    [Command]
    private void CmdCheckCell(Vector2Int cell)
    {
        bool empty = GameManager.Instance.towerManager.CanPlace(cell.x + 24, cell.y + 24);
        isCellEmpty = empty;
    }
    [Command]
    private void CmdSetSelectedCell(Vector2Int cell)
    {
        selectedCell = cell;
    }

}
