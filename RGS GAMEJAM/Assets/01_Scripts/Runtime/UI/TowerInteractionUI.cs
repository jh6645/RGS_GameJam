using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerInteractionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text towerNameTxt;
    [SerializeField] private TMP_Text towerHPTxt;
    [SerializeField] private TMP_Text towerStuffTxt;

    public void SetTower(MainTower MT)
    {
        towerNameTxt.text = "LV" + (MT.towerLevel+1).ToString() + " " + MT.mainTowerData.towerName;
        towerHPTxt.text = MT.towerHealth.currentHP.ToString() + "/" + MT.mainTowerData.towerMaxHP[MT.towerLevel].ToString();
        if (MT.towerLevel <=3)
        {
            towerStuffTxt.text = $"<color=green>{MT.mainTowerData.upgradeStuffs[MT.towerLevel+1].needLeaf}</color>/" +
                $"<color=#925a02>{MT.mainTowerData.upgradeStuffs[MT.towerLevel+1].needStick}</color>/" +
                $"<color=#5b5b5b>{MT.mainTowerData.upgradeStuffs[MT.towerLevel+1].needStone}</color>";
        }
        else
        {
            towerStuffTxt.text = "MAX LEVEL";
        }
    }
}
