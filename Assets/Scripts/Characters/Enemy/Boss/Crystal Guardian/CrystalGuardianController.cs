using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalGuardianController : MonoBehaviour
{
    public float detectionRange = 10f;  // Range within which the enemy detects the player
    public float moveSpeed = 5f;        // Speed at which the enemy moves towards the player
    public float moveDrag = 15f;        // Drag when the enemy is moving
    public float stopDrag = 25f;        // Drag when the enemy stops moving
    public bool isMoving = false;       // Public flag to check if the enemy is moving
    Vector2 direction;
    public bool isAttacking = false;    // Public flag to check if the enemy is attacking
    public float attackRange = 1.5f;    // Range within which the enemy attacks the player
    public EnemyStates currentStateValue; // Current state of the enemy

    private Transform player;  // Reference to the player's transform
    private Rigidbody2D rb;    // Reference to the Rigidbody2D component
    private Animator animator; // Reference to the Animator component
    private Vector2 moveDirection = Vector2.zero; // Direction of movement
    private bool canMove = true; // Flag to check if the enemy can move

    // Define the possible states of the enemy
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
                    rb.drag = stopDrag;
                    canMove = true;
                    break;
                case EnemyStates.WALK:
                    animator.Play("Walk");
                    rb.drag = moveDrag;
                    rb.velocity = direction * moveSpeed;
                    canMove = true;
                    break;
                case EnemyStates.ATTACK:
                    animator.Play("Attack");
                    rb.velocity = Vector2.zero;
                    rb.drag = stopDrag;
                    canMove = false;
                    break;
                case EnemyStates.SPECIAL_1:
                    animator.Play("Special_1");
                    rb.velocity = Vector2.zero;
                    rb.drag = stopDrag;
                    canMove = false;
                    break;
                case EnemyStates.SPECIAL_2:
                    animator.Play("Special_2");
                    rb.velocity = Vector2.zero;
                    rb.drag = stopDrag;
                    canMove = false;
                    break;
                case EnemyStates.SPECIAL_3:
                    animator.Play("Special_3");
                    rb.velocity = Vector2.zero;
                    rb.drag = stopDrag;
                    canMove = false;
                    break;
                case EnemyStates.DIE:
                    animator.Play("Die");
                    rb.velocity = Vector2.zero;
                    rb.drag = stopDrag;
                    canMove = false;
                    break;
            }
        }
    }

    void Start()
    {
        // Find the player object by its tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // Get the Rigidbody2D and Animator components attached to this enemy
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        direction = (player.position - transform.position).normalized;
    }
    void FixedUpdate()
    {
        UpdateAnimationState();
        if (player != null)
        {
            HandleMovementAndState();
        }
    }

    private void HandleMovementAndState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange && canMove && currentStateValue != EnemyStates.DIE)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StopMoving();
        }
        CheckAttackRange(distanceToPlayer);
    }

    private void MoveTowardsPlayer()
    {
        isAttacking = false;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            // Move towards the player if not within attack range
            isMoving = true;
        }
        else
        {
            // Stop moving if within attack range
            isMoving = false;
        }
    }


    private void StopMoving()
    {
        isMoving = false;
        rb.velocity = Vector2.zero;
    }

    // Check if the player is within attack range
    private void CheckAttackRange(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        canMove = false;
        Debug.Log("Attacking player!");
    }

    public void UpdateAnimationState()
    {
        SetEnemyDirection();
        int stateIdentifier;
        if (isAttacking == false)
        {
            if (isMoving)
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

        switch (stateIdentifier)
        {
            case 1:
                CurrentState = EnemyStates.IDLE;
                break;
            case 2:
                CurrentState = EnemyStates.WALK;
                break;
            case 3:
                CurrentState = EnemyStates.ATTACK;
                break;
        }
    }

    private void SetEnemyDirection()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        animator.SetFloat("xMove", direction.x);
        animator.SetFloat("yMove", direction.y);
    }
}
