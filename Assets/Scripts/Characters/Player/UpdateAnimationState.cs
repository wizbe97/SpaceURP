using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateAnimationState : MonoBehaviour
{
    [HideInInspector] public bool stateLock = false;
    public Animator animator;
    private PlayerController playerController;
    private Action action;
    private PlayerAttack playerAttack;
    private Player player;
    public PlayerStates currentStateValue;
    public enum PlayerStates
    {
        IDLE,
        IDLE_HOLDING_GUN,
        IDLE_HOLDING_MELEE,
        WALK,
        WALK_HOLDING_GUN,
        RUN,
        RUN_HOLDING_GUN,
        DASH,
        ATTACK,
        SPEAR_ATTACK,
        DIE
    }

    public void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        action = GetComponent<Action>();
        player = GetComponent<Player>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    public PlayerStates currentState
    {
        set
        {
            if (stateLock == false)
            {
                currentStateValue = value;
                switch (currentStateValue)
                {
                    case PlayerStates.IDLE:
                        animator.Play("Idle");
                        playerController.isDashing = false;
                        stateLock = false;
                        playerController.canMove = true;
                        break;
                    case PlayerStates.IDLE_HOLDING_GUN:
                        animator.Play("Idle_Holding_Gun");
                        playerController.isDashing = false;
                        stateLock = false;
                        playerController.canMove = true;
                        break;
                    case PlayerStates.IDLE_HOLDING_MELEE:
                        animator.Play("Idle_Holding_Melee");
                        playerController.isDashing = false;
                        stateLock = false;
                        playerController.canMove = true;
                        break;
                    case PlayerStates.WALK:
                        animator.Play("Walk");
                        playerController.isDashing = false;
                        stateLock = false;
                        playerController.canMove = true;
                        break;
                    case PlayerStates.WALK_HOLDING_GUN:
                        animator.Play("Walk_Holding_Gun");
                        playerController.isDashing = false;
                        stateLock = false;
                        playerController.canMove = true;
                        break;
                    case PlayerStates.RUN:
                        animator.Play("Run");
                        playerController.isDashing = false;
                        stateLock = false;
                        playerController.canMove = true;
                        break;
                    case PlayerStates.RUN_HOLDING_GUN:
                        animator.Play("Run_Holding_Gun");
                        playerController.isDashing = false;
                        stateLock = false;
                        playerController.canMove = true;
                        break;
                    case PlayerStates.ATTACK:
                        animator.Play("Attack");
                        playerController.isDashing = false;
                        stateLock = true;
                        playerController.canMove = true;
                        break;
                    case PlayerStates.SPEAR_ATTACK:
                        animator.Play("Spear_Attack");
                        playerController.isDashing = false;
                        stateLock = true;
                        playerController.canMove = false;
                        break;
                    case PlayerStates.DASH:
                        animator.Play("Dash");
                        playerController.isDashing = true;
                        stateLock = true;
                        playerController.canMove = true;
                        break;
                    case PlayerStates.DIE:
                        animator.Play("Die");
                        playerController.isDashing = false;
                        stateLock = true;
                        break;
                }
            }
        }
    }

    public void UpdateCharacterAnimationState(Vector2 moveInput)
    {
        int stateIdentifier;

        if (playerController.isDashing)
        {
            stateIdentifier = 7;
        }
        // Check if action and currentItem are not null
        else if (action != null && action.currentItem != null && action.currentItem.itemName == "Spear" && playerAttack.isAttacking == true)
        {
            stateIdentifier = 8;
        }
        else if (playerController.isMoving)
        {
            if (action != null && action.isHoldingWeapon)
            {
                stateIdentifier = playerController.movementSpeed >= 2000 ? 1 : 2;
            }
            else
            {
                stateIdentifier = playerController.movementSpeed >= 2000 ? 3 : 4;
            }
        }
        else if (action != null && action.isHoldingWeapon && playerAttack.isAttacking == false)
        {
            action.CurrentItem();

            if (action.currentItem.itemType == Item.ItemType.GUN)
            {
                stateIdentifier = 5; // IDLE_HOLDING_GUN
            }

            else
            {
                stateIdentifier = 6; // IDLE
            }
        }
        else if (action != null && playerAttack.isAttacking == false)
        {
            action.CurrentItem();

            if (action.currentItem == null)
            {
                stateIdentifier = 6; // IDLE
            }
            else
            {
                stateIdentifier = 9; // IDLE_HOLDING_MELEE
            }
        }
        else
        {
            stateIdentifier = 6;
        }

        switch (stateIdentifier)
        {
            case 1:
                PlayerFollowMouse();
                currentState = PlayerStates.RUN_HOLDING_GUN;
                break;
            case 2:
                PlayerFollowMouse();
                currentState = PlayerStates.WALK_HOLDING_GUN;
                break;
            case 3:
                PlayerFaceMovementDirection();
                currentState = PlayerStates.RUN;
                break;
            case 4:
                PlayerFaceMovementDirection();
                currentState = PlayerStates.WALK;
                break;
            case 5:
                PlayerFollowMouse();
                currentState = PlayerStates.IDLE_HOLDING_GUN;
                break;
            case 6:
                PlayerFaceMovementDirection();
                currentState = PlayerStates.IDLE;
                break;
            case 7:
                PlayerFaceMovementDirection();
                currentState = PlayerStates.DASH;
                break;
            case 8:
                PlayerFollowMouse();
                currentState = PlayerStates.SPEAR_ATTACK;
                break;
            case 9:
                PlayerFaceMovementDirection();
                currentState = PlayerStates.IDLE_HOLDING_MELEE;
                break;
        }

        void PlayerFollowMouse()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 playerToMouse = (mousePosition - (Vector2)transform.position).normalized;
            animator.SetFloat("mouseX", playerToMouse.x);
            animator.SetFloat("mouseY", playerToMouse.y);
        }

        void PlayerFaceMovementDirection()
        {
            Vector2 directionToUse;

            if (playerAttack.hasRecentlyAttacked == true)
                directionToUse = playerAttack.GetAttackDirection();
            else if (playerAttack.hasRecentlyAttacked == false && playerController.moveInput != Vector2.zero)
                directionToUse = playerController.moveInput;
            else
                directionToUse = playerController.lastMoveDirection;

            animator.SetFloat("xMove", directionToUse.x);
            animator.SetFloat("yMove", directionToUse.y);
        }
    }
}