using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalGuardianMovementController : MonoBehaviour
{
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 1.5f;    // Range within which the enemy attacks the player
    public bool isAttacking = false;    // Public flag to check if the enemy is attacking

    [SerializeField] private float moveSpeed = 7500;
    [SerializeField] private float moveDrag = 15f;
    [SerializeField] private float stopDrag = 25f;
    [SerializeField] private bool canMove = true;
    private Rigidbody2D rb;
    private Transform player;  // Reference to the player's transform

    private CrystalGuardian crystalGuardian;
    private CrystalAnimationState crystalAnimationState;
    private Coroutine damageCoroutine;

    [HideInInspector] public bool isMoving = false;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        rb = GetComponent<Rigidbody2D>();
        crystalGuardian = GetComponent<CrystalGuardian>();
        crystalAnimationState = GetComponent<CrystalAnimationState>();
    }

    public bool IsMoving
    {
        set
        {
            isMoving = value;
            crystalAnimationState.UpdateAnimationState();

            if (isMoving)
            {
                rb.drag = moveDrag;
            }
            else
            {
                rb.drag = stopDrag;
            }
        }
    }

    private void FixedUpdate()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            MoveTowardsPlayer();
        }
    }

    public void MoveTowardsPlayer()
    {
        if (!canMove) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        rb.AddForce(moveSpeed * Time.deltaTime * direction, ForceMode2D.Force);
        IsMoving = true;

        if (distanceToPlayer > attackRange)
        {
            // Move towards the player if not within attack range
            IsMoving = true;
        }
        else
        {
            // Stop moving if within attack range
            IsMoving = false;
        }

        CheckAttackRange(distanceToPlayer);
    }

    private void CheckAttackRange(float distanceToPlayer)
    {
        if (!crystalAnimationState.stateLock && distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        if (!crystalAnimationState.stateLock) // Only start the attack if no other state is locked
        {
            canMove = false;
            crystalAnimationState.stateLock = true;
            isAttacking = true;
            Debug.Log("Attacking player!");
            crystalAnimationState.UpdateAnimationState();
        }
    }

    private void OnAttackEnd()
    {
        isAttacking = false;
        canMove = true;
        crystalAnimationState.UpdateAnimationState();
        crystalAnimationState.stateLock = false;
    }

    public void ResetAttack()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
            isAttacking = false;
        }
    }

}
