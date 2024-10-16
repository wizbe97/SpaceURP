using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimationState : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] public Rigidbody2D rb;
    [HideInInspector] protected Transform player;
    protected NPCMovementController npcController;
    protected GameObject playerObject;
    protected NPCDialogue npcDialogue;  // Reference to NPCDialogue script
    public NPCStates currentStateValue;  // Use an enum for states
    [HideInInspector] public bool stateLock = false;  // State lock like in player script
    public bool lookAtPlayer = false;
    public NPC npcType;
    [HideInInspector] public Vector2 direction;
    
    // Add a variable to store the last non-zero movement direction
    private Vector2 lastNonZeroDirection;

    public enum NPC
    {
        SCIENTIST,
        ARENA_GUARD,
        SIDE_CHARACTER
    }

    public enum NPCStates
    {
        IDLE,
        WALK,
        IDLE_BOOK
    }

    public NPCStates CurrentState
    {
        set
        {
            if (!stateLock) // Only update if not locked
            {
                currentStateValue = value;
                SetAnimationState(currentStateValue);
            }
        }
    }

    protected virtual void Start()
    {
        npcController = GetComponent<NPCMovementController>();
        npcDialogue = GetComponentInChildren<NPCDialogue>();
        playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found.");
        }

        // Initialize the lastNonZeroDirection to avoid facing down initially
        lastNonZeroDirection = Vector2.down; // Default initial facing direction
    }

    protected virtual void SetAnimationState(NPCStates state)
    {
        // Default implementation for NPC animation states
        switch (state)
        {
            case NPCStates.IDLE:
                animator.Play("Idle");
                stateLock = false;
                break;
            case NPCStates.WALK:
                animator.Play("Walk");
                stateLock = false;
                break;
            case NPCStates.IDLE_BOOK:
                animator.Play("Idle_Book");
                stateLock = false;
                break;
        }
    }

    public virtual void UpdateNPCAnimationState()
    {
        int stateIdentifier;

        if (rb == null || npcController == null)
            return;

        if (npcController.isMoving == false || npcController.canMove == false && rb.velocity == Vector2.zero)
        {
            stateIdentifier = 0; // IDLE
        }
        else if (npcController.isMoving == true && npcController.canMove == true && rb.velocity != Vector2.zero)
        {
            stateIdentifier = 1; // WALK
        }
        else
        {
            stateIdentifier = 0; // IDLE
        }

        // Set the current state based on the identifier
        switch (stateIdentifier)
        {
            case 0:
                CurrentState = NPCStates.IDLE;
                break;
            case 1:
                CurrentState = NPCStates.WALK;
                break;
        }

        // Update animation direction
        if (lookAtPlayer && player != null || (npcDialogue != null && npcDialogue.speechBubbleRenderer != null && npcDialogue.speechBubbleRenderer.enabled) || npcType == NPC.ARENA_GUARD)
        {
            direction = (player.position - transform.position).normalized;
        }
        else
        {
            // Only update direction if the NPC is moving
            if (rb.velocity.magnitude > 0.1f)
            {
                direction = rb.velocity.normalized;
                lastNonZeroDirection = direction; // Store the last non-zero direction
            }
            else
            {
                direction = lastNonZeroDirection; // Use the last direction when not moving
            }
        }

        SetAnimationDirection(direction);
    }

    public virtual void SetAnimationDirection(Vector2 direction)
    {
        animator.SetFloat("xMove", direction.x);
        animator.SetFloat("yMove", direction.y);
    }
}
