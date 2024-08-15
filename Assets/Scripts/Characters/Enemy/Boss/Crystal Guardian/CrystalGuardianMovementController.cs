using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalGuardianMovementController : MonoBehaviour
{
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float moveSpeed = 7500;
    [SerializeField] private float moveDrag = 15f;
    [SerializeField] private float stopDrag = 25f;
    public bool canMove = true;
    private Rigidbody2D rb;
    [HideInInspector] public Transform player;  // Reference to the player's transform

    private CrystalAnimationState crystalAnimationState;
    private CrystalGuardianAttack crystalGuardianAttack;

    [HideInInspector] public bool isMoving = false;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        rb = GetComponent<Rigidbody2D>();
        crystalAnimationState = GetComponent<CrystalAnimationState>();
        crystalGuardianAttack = GetComponent<CrystalGuardianAttack>();
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
        else
        {
            IsMoving = false;
        }
    }

    public void MoveTowardsPlayer()
    {
        if (!canMove) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if the boss is within detection range
        if (distanceToPlayer <= detectionRange)
        {
            // If the boss is too close to the player, move away
            if (distanceToPlayer < crystalGuardianAttack.attackRange * 0.5f) // Example: 50% of attack range
            {
                // Move away from the player
                Vector2 direction = (transform.position - player.position).normalized;
                rb.AddForce(moveSpeed * Time.deltaTime * direction, ForceMode2D.Force);
                IsMoving = true;
            }
            else if (distanceToPlayer > crystalGuardianAttack.attackRange)
            {
                // Move towards the player if not within attack range
                Vector2 direction = (player.position - transform.position).normalized;
                rb.AddForce(moveSpeed * Time.deltaTime * direction, ForceMode2D.Force);
                IsMoving = true;
            }
            else
            {
                // Stop moving if within attack range
                IsMoving = false;
            }

            // Always check if the boss should attack
            crystalGuardianAttack.CheckAttackRange(distanceToPlayer);
        }
    }


}
