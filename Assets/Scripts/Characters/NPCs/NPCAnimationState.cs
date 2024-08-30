using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimationState : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] public Rigidbody2D rb;
    [HideInInspector] private Transform player;

    private NPCMovementController npcController;
    private NPCDialogue npcDialogue;  // Reference to NPCDialogue script
    public NPCStates currentStateValue;

    public enum NPCStates
    {
        IDLE,
        IDLE_BOOK,
        WALK,
    }

    public NPCStates CurrentState
    {
        set
        {
            currentStateValue = value;
            switch (currentStateValue)
            {
                case NPCStates.IDLE:
                    animator.Play("Idle");
                    break;
                case NPCStates.IDLE_BOOK:
                    animator.Play("Idle_Book");
                    break;
                case NPCStates.WALK:
                    animator.Play("Walk");
                    break;
            }
        }
    }

    private void Awake()
    {
        npcController = GetComponent<NPCMovementController>();
        npcDialogue = GetComponentInChildren<NPCDialogue>();  // Find NPCDialogue script in children
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    public void UpdateAnimationState()
    {
        int stateIdentifier;
        if (npcController.isMoving)
        {
            stateIdentifier = 2;
        }
        else
        {
            stateIdentifier = 1;
        }

        switch (stateIdentifier)
        {
            case 1:
                // If speechBubbleRenderer is enabled, play regular idle
                if (npcDialogue.speechBubbleRenderer.enabled)
                {
                    CurrentState = NPCStates.IDLE;
                }
                else
                {
                    // 50% chance to either be idle or reading a book if speech bubble is not enabled
                    CurrentState = (Random.value < 0.5f) ? NPCStates.IDLE : NPCStates.IDLE_BOOK;
                }

                // Use ternary operator to determine the direction based on speechBubbleRenderer state
                Vector2 direction = npcDialogue.speechBubbleRenderer.enabled ?
                                    (player.position - transform.position).normalized :
                                    rb.velocity.normalized;

                SetAnimationDirection(direction);
                break;

            case 2:
                CurrentState = NPCStates.WALK;
                SetAnimationDirection(rb.velocity.normalized);
                break;
        }
    }



    public void SetAnimationDirection(Vector2 direction)
    {
        // Update animator parameters
        animator.SetFloat("xMove", direction.x);
        animator.SetFloat("yMove", direction.y);
    }
}
