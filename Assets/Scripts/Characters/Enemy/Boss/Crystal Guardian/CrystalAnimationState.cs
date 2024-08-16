using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalAnimationState : MonoBehaviour
{
    private Animator animator; // Reference to the Animator component
    private CrystalGuardian crystalGuardian; // Reference to the CrystalGuardian script
    private CrystalGuardianMovementController crystalGuardianMovementController; // Reference to the CrystalGuardianMovementController script
    private CrystalGuardianAttack crystalGuardianAttack; // Reference to the CrystalGuardianAttack script
    private Transform player; // Reference to the player's transform

    private void Start()
    {
        animator = GetComponent<Animator>();
        crystalGuardian = GetComponent<CrystalGuardian>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        crystalGuardianMovementController = GetComponent<CrystalGuardianMovementController>();
        crystalGuardianAttack = GetComponent<CrystalGuardianAttack>();
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

    public EnemyStates currentStateValue;
    public bool stateLock = false; // True means the state is locked, false means no state is locked

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
        if (currentStateValue == EnemyStates.DIE)
        {
            CurrentState = EnemyStates.DIE;
            return;
        }

        int stateIdentifier = 1; // Default to IDLE

        if (crystalGuardian.isSpecial1)
        {
            stateIdentifier = 4; // SPECIAL_1
        }
        else if (crystalGuardian.isSpecial2)
        {
            stateIdentifier = 5; // SPECIAL_2
        }
        else if (crystalGuardian.isSpecial3)
        {
            stateIdentifier = 6; // SPECIAL_3
        }
        else if (crystalGuardianAttack.isAttacking)
        {
            stateIdentifier = 3; // ATTACK
        }
        else if (crystalGuardianMovementController.isMoving)
        {
            stateIdentifier = 2; // WALK
        }

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
        if (stateLock)
        {
            return;
        }

        Vector2 direction;

        if (crystalGuardian.isSpecial3 && crystalGuardian.Crystal != null)
        {
            direction = (crystalGuardian.Crystal.position - transform.position).normalized;
        }
        else
        {
            direction = (player.position - transform.position).normalized;
        }

        animator.SetFloat("xMove", direction.x);
        animator.SetFloat("yMove", direction.y);
    }
}

