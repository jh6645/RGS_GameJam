using UnityEngine;
using Mirror;
public class Interactor : NetworkBehaviour
{
    private PlayerInputHandler inputHandler;

    [Header("Interact Test")]
    [SerializeField] private Transform interactionPos;
    [SerializeField] private Vector2 interactionRange;
    [SerializeField] private LayerMask interactionLayer;

    [Header("UI")]
    [SerializeField] private GameObject tapPromptPrefab;
    [SerializeField] private GameObject holdPromptPrefab;
    private GameObject promptInstance;

    private IInteractable currentInteractable;
    private IInteractable previousInteractable;

    private InteractionType currentType;
    private bool isHolding = false;
    private float holdTimer = 0f;
    private float requiredHoldTime = 0f;
    public bool isRoomInteract;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        UpdateInteractable();

        if (currentInteractable == null)
        {
            isHolding = false;
            holdTimer = 0f;
            return;
        }

        var type = currentInteractable.GetInteractionType();

        // TAP 贸府
        if (type == InteractionType.Tap)
        {
            if (inputHandler.interactionJustPressed)
            {
                if (isRoomInteract == currentInteractable.isRoomInteractor &&
                    currentInteractable.CanInteract())
                {
                    currentInteractable.Interact(this);
                }
            }
            return;
        }

        // HOLD 贸府
        requiredHoldTime = currentInteractable.GetHoldTime();

        if (inputHandler.isInteractionPressed)
        {
            if (!isHolding)
            {
                isHolding = true;
                holdTimer = 0f;
            }

            holdTimer += Time.deltaTime;

            UpdateHoldProgressUI(holdTimer / requiredHoldTime);

            if (holdTimer >= requiredHoldTime)
            {
                currentInteractable.Interact(this);
                isHolding = false;
                holdTimer = 0f;
            }
        }
        else
        {
            isHolding = false;
            holdTimer = 0f;
            UpdateHoldProgressUI(0f);
        }
    }
    private void UpdateHoldProgressUI(float progress)
    {
        if (promptInstance == null) return;
        if (currentType != InteractionType.Hold) return;

        var ui = promptInstance.GetComponent<InteractionPromptUI_Hold>();
        ui.SetFillAmount(Mathf.Clamp01(progress));
    }
    private void UpdateInteractable()
    {
        if (DoInteractionTest(out IInteractable nearest))
        {
            currentInteractable = nearest;
        }
        else
        {
            currentInteractable = null;
        }

        if (previousInteractable != currentInteractable)
        {
            if (previousInteractable != null)
            {
                previousInteractable.OnExitRange(this);
            }

            if (currentInteractable != null)
            {
                currentInteractable.OnEnterRange(this);
            }

            previousInteractable = currentInteractable;
        }

        if (currentInteractable != null)
        {
            if (!currentInteractable.CanInteract())
            {
                HidePrompt();
            }
            else
            {
                ShowPrompt(currentInteractable);
            }
        }
        else
        {
            HidePrompt();
        }

    }

    private void ShowPrompt(IInteractable target)
    {
        if (!target.CanInteract())
        { 
            HidePrompt(); 
            return; 
        }
        if (isRoomInteract != target.isRoomInteractor)
        {
            HidePrompt();
            return;
        }

        if (!target.isAppearTransform)
        {
            HidePrompt();
            return;
        }

        if (promptInstance == null || currentType != target.GetInteractionType())
        {
            if (promptInstance != null)
                Destroy(promptInstance);

            currentType = target.GetInteractionType();

            if (currentType == InteractionType.Tap)
                promptInstance = Instantiate(tapPromptPrefab);
            else
                promptInstance = Instantiate(holdPromptPrefab);
        }

        promptInstance.SetActive(true);

        Transform appear = target.AppearTransform;
        promptInstance.transform.position = appear.position;

        string text = target.GetPromptText();

        if (currentType == InteractionType.Tap)
        {
            var ui = promptInstance.GetComponent<InteractPromptUI_NonHold>();
            ui?.SetText(text);
        }
        else
        {
            var ui = promptInstance.GetComponent<InteractionPromptUI_Hold>();
            ui?.SetText(text);
        }
    }


    private void HidePrompt()
    {
        if (promptInstance != null)
            promptInstance.SetActive(false);
    }

    private bool DoInteractionTest(out IInteractable interactable)
    {
        interactable = null;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(interactionPos.position, interactionRange, 0, Vector2.right, 0, interactionLayer);

        float closestDist = float.MaxValue;
        for (int i = 0; i < hits.Length; i++)
        {
            float dist = Vector2.Distance(transform.position, hits[i].transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                interactable = hits[i].collider.GetComponent<IInteractable>();
            }
        }

        return interactable != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(interactionPos.position, interactionRange);
    }
}
