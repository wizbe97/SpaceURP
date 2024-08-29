using System.Collections;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [HideInInspector] public SpriteRenderer speechBubbleRenderer;
    public DialogueSO[] conversation;
    private DialogueManager dialogueManager;
    private NPCMovementController npcController;
    private NPCAnimationState npcAnimationState;
    public bool dialogueInitiated;
    public float detectionRadius = 2.0f; // Set this to the desired interaction radius
    public LayerMask playerLayer; // Ensure the Player is on this layer
    public Vector2 detectionOffset = new Vector2(0, -1); // Offset to move detection range down

    private Coroutine movementDelayCoroutine;

    void Start()
    {
        npcAnimationState = GetComponentInParent<NPCAnimationState>();
        npcController = GetComponentInParent<NPCMovementController>();
        speechBubbleRenderer = GetComponent<SpriteRenderer>();
        speechBubbleRenderer.enabled = false;
        dialogueManager = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
    }

    void Update()
    {
        CheckIfPlayerInRange();
        if (dialogueInitiated)
        {
            npcController.canMove = false;
            npcController.isMoving = false;
            npcAnimationState.UpdateAnimationState();
        }
    }

    private void CheckIfPlayerInRange()
    {
        Vector2 detectionCenter = (Vector2)transform.position + detectionOffset;
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionCenter, detectionRadius, playerLayer);

        if (hits.Length > 0 && !dialogueInitiated)
        {
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    speechBubbleRenderer.enabled = true;
                    dialogueManager.InitiateDialogue(this, 0);
                    dialogueInitiated = true;

                    if (movementDelayCoroutine != null)
                    {
                        StopCoroutine(movementDelayCoroutine);
                    }

                    npcController.canMove = false;
                    break;
                }
            }
        }
        else if (dialogueInitiated && hits.Length == 0)
        {
            speechBubbleRenderer.enabled = false;
            dialogueManager.TurnOffDialogue();
            dialogueInitiated = false;

            if (movementDelayCoroutine != null)
            {
                StopCoroutine(movementDelayCoroutine);
            }

            movementDelayCoroutine = StartCoroutine(EnableMovementAfterDelay());
        }
    }

    private IEnumerator EnableMovementAfterDelay()
    {
        yield return new WaitForSeconds(npcController.moveAfterConversationDelay);
        npcController.canMove = true;
    }
}
