using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalGuardianAttack : MonoBehaviour
{
    private BoxCollider2D attackHitbox;    // Reference to the attack hitbox
    public int attackDamage;    // Reference to the attack hitbox
    [SerializeField] private float attackCooldown = 2.0f;  // Time in seconds between attacks
    private float lastAttackTime = 0f;  // Timestamp of the last attack


    public float attackRange = 1.5f;    // Range within which the enemy attacks the player
    public bool isAttacking = false;    // Public flag to check if the enemy is attacking

    private CrystalAnimationState crystalAnimationState;
    private CrystalGuardianMovementController crystalGuardianMovementController;


    private void Start()
    {
        // Initialize the required components
        crystalAnimationState = GetComponent<CrystalAnimationState>();
        crystalGuardianMovementController = GetComponent<CrystalGuardianMovementController>();
        attackHitbox = GetComponentInChildren<BoxCollider2D>();
    }
    public void CheckAttackRange(float distanceToPlayer)
    {
        if (!crystalAnimationState.stateLock && distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            AttackPlayer();
        }
    }


    private void AttackPlayer()
    {
        if (!crystalAnimationState.stateLock) // Only start the attack if no other state is locked
        {
            crystalGuardianMovementController.canMove = false;
            crystalAnimationState.stateLock = true;
            isAttacking = true;
            Debug.Log("Attacking player!");

            // Update the last attack time
            lastAttackTime = Time.time;

            // Determine direction
            Vector2 direction = (crystalGuardianMovementController.player.position - transform.position).normalized;
            UpdateHitboxPosition(direction);

            crystalAnimationState.UpdateAnimationState();
        }
    }


    private void UpdateHitboxPosition(Vector2 direction)
    {
        // Determine which diagonal direction the enemy is facing and adjust the hitbox position and rotation accordingly
        if (direction.x > 0 && direction.y > 0) // Facing up-right
        {
            attackHitbox.transform.localPosition = new Vector3(1.0f, 1.0f, 0);
            attackHitbox.transform.localRotation = Quaternion.Euler(0, 0, 45);
        }
        else if (direction.x < 0 && direction.y > 0) // Facing up-left
        {
            attackHitbox.transform.localPosition = new Vector3(-1.0f, 1.0f, 0);
            attackHitbox.transform.localRotation = Quaternion.Euler(0, 0, 135);
        }
        else if (direction.x > 0 && direction.y < 0) // Facing down-right
        {
            attackHitbox.transform.localPosition = new Vector3(1.0f, -1.0f, 0);
            attackHitbox.transform.localRotation = Quaternion.Euler(0, 0, -45);
        }
        else if (direction.x < 0 && direction.y < 0) // Facing down-left
        {
            attackHitbox.transform.localPosition = new Vector3(-1.0f, -1.0f, 0);
            attackHitbox.transform.localRotation = Quaternion.Euler(0, 0, -135);
        }

    }


    private void OnAttackStart()
    {
        attackHitbox.enabled = true;
    }
    private void OnAttackEnd()
    {
        isAttacking = false;
        crystalGuardianMovementController.canMove = true;
        crystalAnimationState.UpdateAnimationState();
        crystalAnimationState.stateLock = false;
        attackHitbox.enabled = false;
    }

}
