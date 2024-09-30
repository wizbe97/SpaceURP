using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearAttack : MonoBehaviour
{
    private Animator animator;
    private PolygonCollider2D polygonCollider;
    private UpdateAnimationState animationState;
    private PlayerController playerController;
    private PlayerAttack playerAttack;

    void Start()
    {
        animator = GetComponent<Animator>();
        polygonCollider = GetComponentInChildren<PolygonCollider2D>();
        animationState = GetComponentInParent<UpdateAnimationState>();
        playerController = GetComponentInParent<PlayerController>();
        playerAttack = GetComponentInParent<PlayerAttack>();
    }

    public void Attack(float attackDirectionX, float attackDirectionY)
    {
        Debug.Log("Attacking with spear");
        StartCoroutine(EnableColliderAfterDelay(0.15f));  // Start coroutine with 0.5s delay
        animator.SetBool("isAttacking", true);
        animator.SetFloat("mouseX", attackDirectionX);
        animator.SetFloat("mouseY", attackDirectionY);
    }

    private IEnumerator EnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Wait for 0.15 seconds
        polygonCollider.enabled = true;          // Enable the collider after delay
    }

    public void AttackEnd()
    {
        animator.SetBool("isAttacking", false);
        polygonCollider.enabled = false;
        playerAttack.isAttacking = false;
        animationState.stateLock = false;
        animationState.UpdateCharacterAnimationState(playerController.moveInput);
    }
}
