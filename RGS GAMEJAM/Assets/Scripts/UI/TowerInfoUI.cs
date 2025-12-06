using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TowerInfoUI : MonoBehaviour
{
    [SerializeField] private Image towerIcon;
    [SerializeField] private TMP_Text towerName;
    [SerializeField] private TMP_Text towerInfo;
    [SerializeField] private TMP_Text towerStuff;

    public void SetTowerInfo(SO_BaseTower tower)
    {
        towerName.text = tower.towerName;
        towerInfo.text = tower.towerInfo;
        towerIcon.sprite = tower.towerIcon;
        towerStuff.text = $"<color=green>{tower.upgradeStuffs[0].needLeaf}</color>/<color=#925a02>{tower.upgradeStuffs[0].needStick}</color>/<color=#5b5b5b>{tower.upgradeStuffs[0].needStone}</color>";
    }
}
