using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearAttack : MonoBehaviour
{
    private Animator animator;
    private PolygonCollider2D polygonCollider;
    void Start()
    {
        animator = GetComponent<Animator>();
        polygonCollider = GetComponentInChildren<PolygonCollider2D>();
    }

    public void Attack(float attackDirectionX, float attackDirectionY)
    {
        Debug.Log("Attacking with spear");
        polygonCollider.enabled = true;
        animator.SetBool("isAttacking", true);
        animator.SetFloat("mouseX", attackDirectionX);
        animator.SetFloat("mouseY", attackDirectionY);
    }

    public void AttackEnd()
    {
        animator.SetBool("isAttacking", false);
        polygonCollider.enabled = false;
    }
}
