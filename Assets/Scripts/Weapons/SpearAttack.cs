using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearAttack : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Attack(float attackDirectionX, float attackDirectionY)
    {
        Debug.Log("Attacking with spear");
        animator.SetBool("isAttacking", true);
        animator.SetFloat("mouseX", attackDirectionX);
        animator.SetFloat("mouseY", attackDirectionY);
    }

    public void AttackEnd()
    {
        animator.SetBool("isAttacking", false);
    }
}
