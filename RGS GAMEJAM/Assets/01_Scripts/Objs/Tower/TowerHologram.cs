using UnityEngine;

public class TowerHologram : MonoBehaviour
{
    private SO_MainTower mainTowerData;
    public void SetHologram(SO_MainTower MT)
    {
        mainTowerData = MT;

    }
    public void MoveHologram(Vector2Int pos, int rotation)
    {

    }
    public void SetVisibleState(bool active)
    {
        gameObject.SetActive(active);
    }
}
