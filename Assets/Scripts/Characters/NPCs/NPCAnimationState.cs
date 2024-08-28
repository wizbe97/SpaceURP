using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimationState : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb;
    private NPCDialogue npcDialogue;
    private Transform player;

    private NPCController npcController;
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

    private void Start()
    {
        npcController = GetComponent<NPCController>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        npcDialogue = GetComponentInChildren<NPCDialogue>();
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
                CurrentState = NPCStates.IDLE;
                SetAnimationDirection();
                break;

            case 2:
                CurrentState = NPCStates.WALK;
                SetAnimationDirection();
                break;
        }
    }

    private void SetAnimationDirection()
    {
        Vector2 direction;
        direction = rb.velocity.normalized;

        // Update animator parameters
        animator.SetFloat("xMove", direction.x);
        animator.SetFloat("yMove", direction.y);
    }



}
