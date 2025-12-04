using UnityEngine;

public class Interactor : MonoBehaviour
{
    private PlayerInputHandler inputHandler;

    [SerializeField] private Transform interactionPos;
    [SerializeField] private Vector2 interactionRange;
    [SerializeField] private LayerMask interactionLayer;
    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
    }
    private void Update()
    {
        if (inputHandler.interactionJustPressed)
        {
            if(DoInteractionTest(out IInteractable interactable))
            {
                if (interactable.CanInteract())
                {
                    interactable.Interact(this);
                }
            }
        }
    }
    private bool DoInteractionTest(out IInteractable interactable)
    {
        interactable = null;
        RaycastHit2D[] hitObjs = Physics2D.BoxCastAll(interactionPos.position, interactionRange, 0, Vector2.right, 0, interactionLayer);

        float distance = 999f;
        for(int i = 0; i < hitObjs.Length; i++)
        {
            float dist = Vector2.Distance(transform.position, hitObjs[i].transform.position);
            if (dist < distance)
            {
                interactable = hitObjs[i].collider.GetComponent<IInteractable>();
            }
        }
        if (interactable != null) return true;
        return false;
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(interactionPos.position, interactionRange);
    }
}
