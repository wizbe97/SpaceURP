using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalAnimationState : MonoBehaviour
{
    [HideInInspector] public Animator animator; // Reference to the Animator component
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
        SPECIAL_1_START,
        SPECIAL_1_LOOP,
        SPECIAL_2_START,
        SPECIAL_2_LOOP,
        SPECIAL_3_LOOP,
        SPECIAL_3_START,
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
                case EnemyStates.SPECIAL_1_START:
                    animator.Play("Special1_Start");
                    crystalGuardianMovementController.canMove = false;
                    break;
                case EnemyStates.SPECIAL_1_LOOP:
                    animator.Play("Special1_Loop");
                    crystalGuardianMovementController.canMove = false;
                    break;
                case EnemyStates.SPECIAL_2_START:
                    animator.Play("Special2_Start");
                    crystalGuardianMovementController.canMove = false;
                    break;
                case EnemyStates.SPECIAL_2_LOOP:
                    animator.Play("Special2_Loop");
                    crystalGuardianMovementController.canMove = false;
                    break;
                case EnemyStates.SPECIAL_3_LOOP:
                    animator.Play("Special3_Loop");
                    crystalGuardianMovementController.canMove = false;
                    break;
                case EnemyStates.SPECIAL_3_START:
                    animator.Play("Special3_Start");
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
            SetCurrentState(EnemyStates.DIE);
            return;
        }

        EnemyStates newState = DetermineState();
        SetCurrentState(newState);
    }

    private EnemyStates DetermineState()
    {
        if (crystalGuardian.isSpecial1)
        {
            return animator.GetBool("specialLoop") ? EnemyStates.SPECIAL_1_LOOP : EnemyStates.SPECIAL_1_START;
        }
        if (crystalGuardian.isSpecial2)
        {
            return animator.GetBool("specialLoop") ? EnemyStates.SPECIAL_2_LOOP : EnemyStates.SPECIAL_2_START;
        }
        if (crystalGuardian.isSpecial3)
        {
            return animator.GetBool("specialLoop") ? EnemyStates.SPECIAL_3_LOOP : EnemyStates.SPECIAL_3_START;
        }
        if (crystalGuardianAttack.isAttacking)
        {
            return EnemyStates.ATTACK;
        }
        if (crystalGuardianMovementController.isMoving)
        {
            return EnemyStates.WALK;
        }

        return EnemyStates.IDLE;
    }

    private void SetCurrentState(EnemyStates newState)
    {
        if (newState != EnemyStates.DIE)
        {
            SetEnemyDirection();
        }
        CurrentState = newState;
    }


    public void SetEnemyDirection()
    {
        if (stateLock)
        {
            return;
        }

        Vector2 direction;

        if (crystalGuardian.isSpecial3 && crystalGuardian.Crystal != null)
        {
            direction = (crystalGuardian.Crystal.position - transform.position).normalized;
            direction.x = direction.x >= 0 ? 1 : -1;
            direction.y = direction.y >= 0 ? 1 : -1;
        }
        else
        {
            direction = (player.position - transform.position).normalized;
        }

        animator.SetFloat("xMove", direction.x);
        animator.SetFloat("yMove", direction.y);
    }

    private void SetBoolTrue()
    {

        animator.SetBool("specialLoop", true);
        if (crystalGuardian.isSpecial1)
        {
            StartCoroutine(SpecialRoutine());
            crystalGuardian.SetAbilityStates(true, false, false);
        }

        if (crystalGuardian.isSpecial2)
        {
            StartCoroutine(SpecialRoutine());
            crystalGuardian.SetAbilityStates(false, true, false);
        }

        if (crystalGuardian.isSpecial3)
        {
            StartCoroutine(SpecialRoutine());
            crystalGuardian.SetAbilityStates(false, false, true);
        }

    }

    private IEnumerator SpecialRoutine()
    {
        if (crystalGuardian.isSpecial1 && !crystalGuardian.isSpecial1Active)
        {
            yield return crystalGuardian.SpawnSpikes();
            crystalGuardian.ResetAbilityStates();
        }
        else if (crystalGuardian.isSpecial2 && !crystalGuardian.isSpecial2Active)
        {
            yield return crystalGuardian.RockFallRoutine();
            crystalGuardian.ResetAbilityStates();
        }
        else if (crystalGuardian.isSpecial3 && !crystalGuardian.isSpecial3Active)
        {
            yield return crystalGuardian.LaserRoutine();
            crystalGuardian.ResetAbilityStates();
        }
    }

    public void PlayEndAnimation()
    {
        if (crystalGuardian.isSpecial1)
        {
            Debug.Log("No End animation");
        }
        else if (crystalGuardian.isSpecial2)
        {
            animator.Play("Special2_End");
        }
        else if (crystalGuardian.isSpecial3)
        {
            Debug.Log("No End animation");
        }

    }

}

