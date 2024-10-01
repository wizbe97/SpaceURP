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
        WALK_HOLDING_MELEE,
        RUN,
        RUN_HOLDING_GUN,
        RUN_HOLDING_MELEE,
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
                    case PlayerStates.WALK_HOLDING_MELEE:
                        animator.Play("Walk_Holding_Melee");
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
                    case PlayerStates.RUN_HOLDING_MELEE:
                        animator.Play("Run_Holding_Melee");
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
        int stateIdentifier = 1;

        if (playerController.isDashing) // DASHING
        {
            stateIdentifier = 10; // DASH
        }
        else if (playerAttack.isAttacking) // ATTACKING
        {
            if (action != null && action.currentItem.itemName == "Spear")
            {
                stateIdentifier = 11; // SPEAR_ATTACK
            }
            else
            {
                stateIdentifier = 12; // ATTACK
            }
        }
        else if (playerController.isMoving) // WALK OR RUN
        {
            if (action != null && action.currentItem != null)
            {
                if (action.currentItem.itemType == Item.ItemType.MELEE_WEAPON)
                {
                    stateIdentifier = playerController.movementSpeed > 2000 ? 9 : 4; // RUN_HOLDING_MELEE : WALK_HOLDING_MELEE
                }
                else if (action.currentItem.itemType == Item.ItemType.GUN)
                {
                    stateIdentifier = playerController.movementSpeed > 2000 ? 8 : 5; // RUN_HOLDING_GUN : WALK_HOLDING_GUN
                }
            }
            else // No item is held
            {
                stateIdentifier = playerController.movementSpeed > 2000 ? 7 : 4; // RUN : WALK
            }
        }

        else // IDLE STATES
        {
            if (action != null && action.currentItem != null)
            {
                if (action.currentItem.itemType == Item.ItemType.MELEE_WEAPON)
                {
                    stateIdentifier = 3; // IDLE_HOLDING_MELEE
                }
                else if (action.currentItem.itemType == Item.ItemType.GUN)
                {
                    stateIdentifier = 2; // IDLE_HOLDING_GUN
                }
            }
            else
            {
                stateIdentifier = 1; // IDLE
            }
        }

        switch (stateIdentifier)
        {
            case 1:
                PlayerFaceMovementDirection();
                currentState = PlayerStates.IDLE;
                break;
            case 2:
                PlayerFollowMouse();
                currentState = PlayerStates.IDLE_HOLDING_GUN;
                break;
            case 3:
                PlayerFaceMovementDirection();
                currentState = PlayerStates.IDLE_HOLDING_MELEE;
                break;
            case 4:
                PlayerFaceMovementDirection();
                currentState = PlayerStates.WALK;
                break;
            case 5:
                PlayerFollowMouse();
                currentState = PlayerStates.WALK_HOLDING_GUN;
                break;
            // case 6:
            //     PlayerFollowMouse();
            //     currentState = PlayerStates.WALK_HOLDING_MELEE;
            //     break;
            case 7:
                PlayerFaceMovementDirection();
                currentState = PlayerStates.RUN;
                break;
            case 8:
                PlayerFollowMouse();
                currentState = PlayerStates.RUN_HOLDING_GUN;
                break;
            case 9:
                PlayerFaceMovementDirection();
                currentState = PlayerStates.RUN_HOLDING_MELEE;
                break;
            case 10:
                PlayerFaceMovementDirection();
                currentState = PlayerStates.DASH;
                break;
            case 11:
                PlayerFollowMouse();
                currentState = PlayerStates.SPEAR_ATTACK;
                break;
            case 12:
                PlayerFollowMouse();
                currentState = PlayerStates.ATTACK;
                break;
        }

        void PlayerFollowMouse()
        {
            if (stateLock == true)
                return;
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