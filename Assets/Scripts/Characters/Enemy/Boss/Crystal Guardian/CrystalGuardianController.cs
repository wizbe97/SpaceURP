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

    private Transform player;  // Reference to the player's transform
    private Rigidbody2D rb;    // Reference to the Rigidbody2D component
    private Vector2 moveDirection = Vector2.zero; // Direction of movement
    public bool canMove = true; // Flag to check if the enemy can move
    public bool stateLock = false; // True means the state is locked, false means no state is locked

    private CrystalGuardian crystalGuardian; // Reference to the CrystalGuardian script
    private CrystalAnimationState crystalAnimationState;

    // Define the possible states of the enemy

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
        crystalGuardian = GetComponent<CrystalGuardian>();
    }

    void Update()
    {
        direction = (player.position - transform.position).normalized;
    }
    void FixedUpdate()
    {
        if (player != null)
        {
            HandleMovementAndState();
        }
    }

    private void HandleMovementAndState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (!stateLock && crystalGuardian.isSpecial1 == false && crystalGuardian.isSpecial2 == false && crystalGuardian.isSpecial3 == false && distanceToPlayer <= detectionRange && crystalAnimationState.currentStateValue != CrystalAnimationState.EnemyStates.DIE)
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
        if (!stateLock) // Only start the attack if no other state is locked
        {
            canMove = false;
            stateLock = true;
            isAttacking = true;
            Debug.Log("Attacking player!");

        }
    }

}
