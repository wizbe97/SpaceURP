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
    public float detectionRadius = 2.0f;
    public LayerMask playerLayer;
    public Vector2 detectionOffset = new Vector2(0, -1);

    private Coroutine movementDelayCoroutine;
    private Coroutine checkInRangeCoroutine;
    private bool playerInTrigger;

    void Start()
    {
        npcAnimationState = GetComponentInParent<NPCAnimationState>();
        npcController = GetComponentInParent<NPCMovementController>();
        speechBubbleRenderer = GetComponent<SpriteRenderer>();
        speechBubbleRenderer.enabled = false;
        dialogueManager = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            if (checkInRangeCoroutine == null)
            {
                checkInRangeCoroutine = StartCoroutine(CheckPlayerInRangeCoroutine(0.1f)); // Start checking every 0.2 seconds
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited trigger.");
            playerInTrigger = false;
            if (checkInRangeCoroutine != null)
            {
                StopCoroutine(checkInRangeCoroutine);
                checkInRangeCoroutine = null;
            }

            if (dialogueInitiated)
            {
                EndDialogue();
            }
        }
    }

    private IEnumerator CheckPlayerInRangeCoroutine(float interval)
    {
        while (playerInTrigger)
        {
            Debug.Log("Checking if player is in range.");
            CheckIfPlayerInRange();
            yield return new WaitForSeconds(interval);
        }
    }

    private void CheckIfPlayerInRange()
    {
        Vector2 detectionCenter = (Vector2)transform.position + detectionOffset;
        Collider2D[] hits = Physics2D.OverlapBoxAll(detectionCenter, new Vector2(detectionRadius, detectionRadius), 0, playerLayer);

        if (hits.Length > 0)
        {
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    Debug.Log("Player detected within range.");
                    speechBubbleRenderer.enabled = true;
                    npcAnimationState.UpdateAnimationState();
                    if (!dialogueInitiated)
                    {
                        StartDialogue();
                    }
                    break;
                }
            }
        }
        else
        {
            if (dialogueInitiated)
            {
                Debug.Log("Player left range.");
                EndDialogue();
            }
        }
    }

    private void StartDialogue()
    {
        Debug.Log("Starting dialogue.");
        dialogueManager.InitiateDialogue(this, 0);
        dialogueInitiated = true;

        if (movementDelayCoroutine != null)
        {
            StopCoroutine(movementDelayCoroutine);
        }

        npcController.canMove = false;
    }

    private void EndDialogue()
    {
        Debug.Log("Ending dialogue.");
        speechBubbleRenderer.enabled = false;
        dialogueManager.TurnOffDialogue();
        dialogueInitiated = false;

        if (movementDelayCoroutine != null)
        {
            StopCoroutine(movementDelayCoroutine);
        }

        movementDelayCoroutine = StartCoroutine(EnableMovementAfterDelay());
    }

    private IEnumerator EnableMovementAfterDelay()
    {
        yield return new WaitForSeconds(npcController.moveAfterConversationDelay);
        npcController.canMove = true;
    }
}
