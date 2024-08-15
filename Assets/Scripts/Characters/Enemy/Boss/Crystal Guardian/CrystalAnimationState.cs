using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalAnimationState : MonoBehaviour
{
    private Animator animator; // Reference to the Animator component

    public EnemyStates currentStateValue;
    public bool stateLock = false; // True means the state is locked, false means no state is locked


    private CrystalGuardian crystalGuardian; // Reference to the CrystalGuardian script
    private CrystalGuardianMovementController crystalGuardianMovementController;
    private CrystalGuardianAttack crystalGuardianAttack;

    private Transform player;  // Reference to the player's transform


    private void Start()
    {
        animator = GetComponent<Animator>();
        crystalGuardian = GetComponent<CrystalGuardian>();
        crystalGuardianMovementController = GetComponent<CrystalGuardianMovementController>();
        crystalGuardianAttack = GetComponent<CrystalGuardianAttack>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }
    public enum EnemyStates
    {
        IDLE,
        WALK,
        ATTACK,
        SPECIAL_1,
        SPECIAL_2,
        SPECIAL_3,
        DIE
    }

    // Property to manage the current state and trigger corresponding actions
    public EnemyStates CurrentState
    {
        set
        {
            currentStateValue = value;
            switch (currentStateValue)
            {
                case EnemyStates.IDLE:
                    animator.Play("Idle");
                    crystalGuardianMovementController.canMove = true;
                    break;
                case EnemyStates.WALK:
                    animator.Play("Walk");
                    crystalGuardianMovementController.canMove = true;
                    break;
                case EnemyStates.ATTACK:
                    animator.Play("Attack");
                    crystalGuardianMovementController.canMove = false;
                    break;
                case EnemyStates.SPECIAL_1:
                    animator.Play("Special1");
                    crystalGuardianMovementController.canMove = false;
                    break;
                case EnemyStates.SPECIAL_2:
                    animator.Play("Special2");
                    crystalGuardianMovementController.canMove = false;
                    break;
                case EnemyStates.SPECIAL_3:
                    animator.Play("Special3");
                    crystalGuardianMovementController.canMove = false;
                    break;
                case EnemyStates.DIE:
                    animator.Play("Die");
                    crystalGuardianMovementController.canMove = false;
                    break;
            }
        }
    }

    public void UpdateAnimationState()
    {

        // Check if the enemy is dead first
        if (currentStateValue == EnemyStates.DIE)
        {
            CurrentState = EnemyStates.DIE;
            return;
        }

        // Set the direction of the enemy (for animation purposes)

        int stateIdentifier;

        // Check for special abilities
        // if (crystalGuardian.isSpecial1)
        // {
        //     stateIdentifier = 4; // SPECIAL_1
        // }
        // else if (crystalGuardian.isSpecial2)
        // {
        //     stateIdentifier = 5; // SPECIAL_2
        // }
        // else if (crystalGuardian.isSpecial3)
        // {
        //     stateIdentifier = 6; // SPECIAL_3
        // }
        if (crystalGuardianAttack.isAttacking == false)
        {
            if (crystalGuardianMovementController.isMoving)
            {
                stateIdentifier = 2;
            }
            else
            {
                stateIdentifier = 1;
            }
        }
        else
        {
            stateIdentifier = 3;
        }


        // Update the current state based on the determined stateIdentifier
        switch (stateIdentifier)
        {
            case 1:
                SetEnemyDirection();
                CurrentState = EnemyStates.IDLE;
                break;
            case 2:
                SetEnemyDirection();
                CurrentState = EnemyStates.WALK;
                break;
            case 3:
                SetEnemyDirection();
                CurrentState = EnemyStates.ATTACK;
                break;
            case 4:
                SetEnemyDirection();
                CurrentState = EnemyStates.SPECIAL_1;
                break;
            case 5:
                SetEnemyDirection();
                CurrentState = EnemyStates.SPECIAL_2;
                break;
            case 6:
                SetEnemyDirection();
                CurrentState = EnemyStates.SPECIAL_3;
                break;
            case 7:
                CurrentState = EnemyStates.DIE; // This case is already handled at the start
                break;
        }
    }


    private void SetEnemyDirection()
    {
        // If stateLock is true, do not update the direction
        if (stateLock)
        {
            return;
        }

        // Calculate and set the direction for the animation
        Vector2 direction = (player.position - transform.position).normalized;
        animator.SetFloat("xMove", direction.x);
        animator.SetFloat("yMove", direction.y);
    }
}
