using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using UnityEngine.Windows;
public class PlayerTower : NetworkBehaviour
{
    [SerializeField] private Image towerImg;
    [SerializeField] private TMP_Text towerAmountTxt;
    [SyncVar(hook =nameof(OnTowerAmountChanged))] public int holdingTowerAmount = 0;
    [SyncVar(hook = nameof(OnTowerChanged))] public MainTowerType nowTowerType;
    [SyncVar] public bool isCellEmpty = false;

    private Vector2Int occupiedCell;
    private Vector2Int selectedCell;
    private Vector2Int lastSentCell = new Vector2Int(0, 0);

    [SerializeField] private GameObject playerFollowingPrefab;
    private GameObject towerInteractionTrigger;
    private PlayerInputHandler playerInputHandler;

    public bool isCellOutOfRange;
    public bool isMovingTower => holdingTowerAmount > 0;

    private int currentRotation;
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
            HandleRotation();
        }
        else
        {
            GameManager.Instance.gridRenderer.UnHighlightCell();
        }
    }
    private void HandleRotation()
    {
        float wheel = playerInputHandler.zoomInput;

        if (Mathf.Abs(wheel) < 0.01f) return;

        if (wheel > 0f)
        {
            currentRotation = (currentRotation + 1) % 4;
        }
        else
        {
            currentRotation = (currentRotation + 3) % 4;
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
    public void AddTower(SO_MainTower BT)
    {
        if (isMovingTower) return;
        if (!isEnoughResources(BT)) return;
        CmdUseResources(BT.upgradeStuffs[0].needLeaf, BT.upgradeStuffs[0].needStick, BT.upgradeStuffs[0].needStone);
        towerInteractionTrigger.SetActive(true);
        GameManager.Instance.gridRenderer.ToggleGrid(true);
        towerInteractionTrigger.GetComponent<TowerPlaceInteraction>().SetTowerData(BT);
        CmdAddTower(BT.towerType, BT.craftingAmount);
    }
    private bool isEnoughResources(SO_MainTower BT)
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
    private void CmdAddTower(MainTowerType towerType, int towerAmount) 
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
        GameManager.Instance.towerManager.PlaceTower(towerPos.x, towerPos.y, nowTowerType, currentRotation);
        holdingTowerAmount -= 1;
        if (!isMovingTower)
        {
            nowTowerType = MainTowerType.NONE;
        }
    }


    private void OnTowerChanged(MainTowerType oldValue, MainTowerType newValue)
    {
        if (newValue!=MainTowerType.NONE)
        {
            towerImg.sprite = GameManager.Instance.towerManager.towerDataDict[newValue].towerIcon;
        }
        else
        {
            towerImg.sprite = null;
        }
        isTowerImgShowing(newValue != MainTowerType.NONE);
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
        if (nowTowerType == MainTowerType.NONE)
        {
            return false;
        }
        //이줄에서 클라이언트 타워 설치 시 Authority Warning이 남. 수정 필요
        CmdCheckCell(selectedCell, GameManager.Instance.towerManager.towerDataDict[nowTowerType].cellPos, currentRotation);
        return isCellEmpty;
    }
    [Command]
    private void CmdCheckCell(Vector2Int origin, Vector2Int[] shape, int rotation)
    {
        bool empty = GameManager.Instance.towerManager.CanPlace(origin, shape, rotation);
        isCellEmpty = empty;
    }
    [Command]
    private void CmdSetSelectedCell(Vector2Int cell)
    {
        selectedCell = cell;
    }

}
