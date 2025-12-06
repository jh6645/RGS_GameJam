using UnityEngine;
using Mirror;
using System.Collections;
public class MainTree : NetworkBehaviour
{
    [SerializeField] private SO_Tree treeData;
    [SerializeField] private GameObject treeReadyParticle;
    [SyncVar] public int TreeLevel;
    [SyncVar] public int NowXp;
    [SyncVar] public int NowHealth;

    [SyncVar] private double lastInteractTime = 0;
    [SyncVar] private float interactCooldown;

    private Coroutine leafRoutine;
    private void Awake()
    {

    }

    private void Start()
    {

    }
    public double GetLastInteractionTime()
    {
        return lastInteractTime;
    }
    public float GetInteractionCoolDown()
    {
        return interactCooldown;
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        TreeLevel = 0;
        NowHealth = treeData.maxHps[TreeLevel];
        NowXp = 0;
        interactCooldown = treeData.stickProduceCool[TreeLevel];

        leafRoutine = StartCoroutine(leafCoroutine());
    }
    private void Update()
    {
        if (NetworkTime.time > interactCooldown + lastInteractTime)
        {
            treeReadyParticle.SetActive(true);
        }
        else
        {
            treeReadyParticle.SetActive(false);
        }
    }

    [Server]
    public void ServerTryGetStick(CustomNetworkGamePlayer player)
    {
        if (NetworkTime.time < lastInteractTime + interactCooldown)
            return;

        lastInteractTime = NetworkTime.time;

        player.AddLocalStick(treeData.stickAmount[TreeLevel]);
    }
    [Server]
    public void AddXp(int amount)
    {
        NowXp += amount;
        if (NowXp >= treeData.needXps[TreeLevel])
        {
            TreeLevelUp();
        }
    }
    [Server]
    private IEnumerator leafCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(treeData.leafProduceCool[TreeLevel]);
            GameManager.Instance.resourceManager.ServerAddLeaf(treeData.leafAmount[TreeLevel]);
        }
    }
    private void TreeLevelUp()
    {
        NowXp -= treeData.needXps[TreeLevel];
        TreeLevel++;
        interactCooldown = treeData.stickProduceCool[TreeLevel];
    }

}
