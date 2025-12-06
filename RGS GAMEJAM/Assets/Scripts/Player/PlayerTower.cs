using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class PlayerTower : NetworkBehaviour
{
    [SerializeField] private Image towerImg;
    public bool isMovingTower = false;

    private void Update()
    {
        if (!isLocalPlayer) return;
        if (isMovingTower)
        {
            Vector3 playerPos = transform.position;

            int cellX = Mathf.FloorToInt(playerPos.x / GameManager.Instance.gridRenderer.cellSize);
            int cellY = Mathf.FloorToInt(playerPos.z / GameManager.Instance.gridRenderer.cellSize);
            GameManager.Instance.gridRenderer.HighlightCell(cellX, cellY);

        }
    }
    public void AddTower(BaseTower BT)
    {
        GameManager.Instance.gridRenderer.ToggleGrid(true);
    }
}
