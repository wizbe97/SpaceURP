using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [HideInInspector] public SpriteRenderer speechBubbleRenderer;
    public DialogueSO[] conversation;
    private DialogueManager dialogueManager;
    private NPCController npcController;
    private NPCAnimationState npcAnimationState;
    public bool dialogueInitiated;
    public float detectionRadius = 2.0f; // Set this to the desired interaction radius
    public LayerMask playerLayer; // Ensure the Player is on this layer
    public Vector2 detectionOffset = new Vector2(0, -1); // Offset to move detection range down

    void Start()
    {
        npcAnimationState = GetComponentInParent<NPCAnimationState>();
        npcController = GetComponentInParent<NPCController>();
        speechBubbleRenderer = GetComponent<SpriteRenderer>();
        speechBubbleRenderer.enabled = false;
        dialogueManager = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfPlayerInRange();
        if (dialogueInitiated)
        {
            npcController.canMove = false;
            npcController.isMoving = false;
            npcAnimationState.UpdateAnimationState();

        }
        else
        {
            npcController.canMove = true;
            npcController.isMoving = true;
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
                    break;
                }
            }
        }
        else if (dialogueInitiated && hits.Length == 0)
        {
            speechBubbleRenderer.enabled = false;
            dialogueManager.TurnOffDialogue();
            dialogueInitiated = false;
        }
    }

}
