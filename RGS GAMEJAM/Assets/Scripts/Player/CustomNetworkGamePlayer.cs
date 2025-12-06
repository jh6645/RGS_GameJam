using UnityEngine;
using Mirror;
public class CustomNetworkGamePlayer : NetworkBehaviour
{
    public static CustomNetworkGamePlayer localPlayer;

    [SyncVar(hook = nameof(OnCharacterChanged))]
    [SerializeField] private Character currentCharacter;

    [SerializeField] private RuntimeAnimatorController[] characterACs;

    [SyncVar(hook = nameof(OnStickChanged))] private int localStick = 0;
    [SyncVar(hook = nameof(OnStoneChanged))] private int localStone = 0;

    private Animator playerAnimator;
    private PlayerTower playerTower;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerTower = GetComponent<PlayerTower>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        ChangeCharacterAC((int)currentCharacter);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CharacterChange(((CustomNetworkRoomManager)NetworkManager.singleton).GetCharacter());
        localPlayer = this;
    }

    private void OnCharacterChanged(Character oldValue, Character newValue)
    {
        currentCharacter = newValue;

        if (isLocalPlayer) return;
        ChangeCharacterAC((int)newValue);
    }

    public void CharacterChange(Character character)
    {
        if (!isLocalPlayer) return;

        ChangeCharacterAC((int)character);
        CmdSendCharacter(character);
    }

    [Command]
    private void CmdSendCharacter(Character character)
    {
        currentCharacter = character;
    }

    private void ChangeCharacterAC(int index)
    {
        if (characterACs[index] != null)
        {
            playerAnimator.runtimeAnimatorController = characterACs[index];
        }
        else
        {
            Debug.LogError("No AC for that Character");
        }
    }
    [Command]
    public void CmdTryGetStick()
    {
        GameManager.Instance.mainTree.ServerTryGetStick(this);
    }
    public void AddLocalStick(int amount)
    {
        localStick += amount;
    }
    public void RemoveLocalStick(int amount)
    {
        localStick -= amount;
    }
    public void AddLocalStone(int amount)
    {
        localStone += amount;
    }
    public void RemoveLocalStone(int amount)
    {
        localStone -= amount;
    }
    private void OnStickChanged(int oldValue, int newValue)
    {
        if (!isLocalPlayer) return;

        GameManager.Instance.resourceManager.localStickAmountTxt.text = $"({newValue})";
    }

    private void OnStoneChanged(int oldValue, int newValue)
    {
        if (!isLocalPlayer) return;

        GameManager.Instance.resourceManager.localStoneAmountTxt.text = $"({newValue})";
    }

    [Command]
    public void CmdMoveResourceToGlobal()
    {
        ServerMoveResourceToGlobal();
    }
    [Server]
    private void ServerMoveResourceToGlobal()
    {
        if (localStick > 0)
        {
            localStick--;
            GameManager.Instance.resourceManager.ServerAddStick(1);
        }

        if (localStone > 0)
        {
            localStone--;
            GameManager.Instance.resourceManager.ServerAddStone(1);
        }
    }
}
