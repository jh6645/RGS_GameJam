using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Unity.Services.Lobbies.Models;
public class MainTowerHealth : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHPAmountChanged))] public int currentHP;

    [SerializeField] private Image towerHPImg;
    private MainTower MT;
    private bool _isDead;
    public bool isDead { get => _isDead; set => _isDead = value; }

    private void Awake()
    {
        MT = GetComponent<MainTower>();
    }

    [Server]
    public void Init()
    {
        currentHP = MT.mainTowerData.towerMaxHP[0];
        isDead = false;
    }

    [Server]
    public void MainTakeDamage(float damage)
    {
        if (isDead) return;

        currentHP -= Mathf.CeilToInt(damage);

        if (currentHP <= 0) {
            isDead = true;
            OnTowerDestroy();
        }
    }

    [Server]
    public void OnTowerDestroy()
    {
        MT.RemoveAllTower();
        MT.BPO.ServerDespawn();
    }
    [Server]
    public void SetCurrentHP(int amount)
    {
        currentHP = 0;
        currentHP = amount;
    }
    private void OnHPAmountChanged(int oldValue, int newValue)
    {
        towerHPImg.fillAmount = (float)newValue / MT.mainTowerData.towerMaxHP[MT.towerLevel];
        MT.towerInteraction.SetTower();
    }
}
