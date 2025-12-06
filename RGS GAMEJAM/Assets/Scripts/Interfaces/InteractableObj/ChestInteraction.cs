using UnityEngine;
using System.Collections;
public class ChestInteraction : MonoBehaviour, IInteractable
{
    [Header("UI Prompt")]
    [SerializeField] private Transform appearTransform;
    [SerializeField] private bool useAppearTransform = true;

    [SerializeField] private float moveSpeed = 0.5f;
    public Transform AppearTransform => useAppearTransform && appearTransform != null ? appearTransform : transform;
    public bool isAppearTransform => useAppearTransform && appearTransform != null;
    public bool isRoomInteractor => false;
    public InteractionType GetInteractionType() => InteractionType.Tap;
    public float GetHoldTime() => 0f;
    public string GetPromptText() => "상자 열기";

    private bool isInRange = false;
    private Interactor currentInteractor;
    private Coroutine transferRoutine;


    private void Update()
    {
        if (isInRange)
        {

        }
    }
    public bool CanInteract()
    {
        return true;
    }

    public bool Interact(Interactor interactor)
    {
        return true;
    }

    public void OnEnterRange(Interactor interactor)
    {
        isInRange = true;
        currentInteractor = interactor;

        if (interactor.isLocalPlayer)
        {
            transferRoutine = StartCoroutine(AutoTransferRoutine());
        }
    }

    public void OnExitRange(Interactor interactor)
    {
        isInRange = false;

        if (interactor.isLocalPlayer && transferRoutine != null)
        {
            StopCoroutine(transferRoutine);
            transferRoutine = null;
        }
        currentInteractor = null;
    }
    private IEnumerator AutoTransferRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveSpeed);

            if (!isInRange) break;

            CustomNetworkGamePlayer.localPlayer.CmdMoveResourceToGlobal();
        }
    }
}
