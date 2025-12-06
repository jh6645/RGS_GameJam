using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    [Header("UI Prompt")]
    [SerializeField] private Transform appearTransform; // UI가 뜰 위치 지정
    [SerializeField] private bool useAppearTransform = true;

    [SerializeField] private GameObject magicDoorPanel;
    public Transform AppearTransform => useAppearTransform && appearTransform != null ? appearTransform : transform;
    public bool isAppearTransform => useAppearTransform && appearTransform != null;
    public bool isRoomInteractor => true;
    public InteractionType GetInteractionType() => InteractionType.Tap;
    public float GetHoldTime() => 0f;
    public string GetPromptText() => "문 열기";
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
        if (magicDoorPanel != null) {
            magicDoorPanel.SetActive(true);
        }

    }

    public void OnExitRange(Interactor interactor)
    {
        if (magicDoorPanel != null)
        {
            magicDoorPanel.SetActive(false);
        }
    }
}
