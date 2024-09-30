using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimations : MonoBehaviour
{
    private Animator animator;


    public WeaponStates currentStateValue;
    private PlayerController playerController;
    private PlayerAttack playerAttack;
    public enum WeaponStates
    {
        IDLE,
        ATTACK
    }

    public WeaponStates CurrentState
    {
        set
        {
            currentStateValue = value;
            switch (currentStateValue)
            {
                case WeaponStates.IDLE:
                    animator.Play("Idle");
                    break;
                case WeaponStates.ATTACK:
                    animator.Play("Attack");
                    break;
            }
        }
    }


    private void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();
        playerAttack = GetComponentInParent<PlayerAttack>();
    }

    private void Update()
    {
        UpdateAnimationWeapon();
    }
    void UpdateAnimationWeapon()
    {
        if (playerAttack.isAttacking == true)
        {
            CurrentState = WeaponStates.ATTACK;
            WeaponFollowMouse();
        }
        else
        {
            CurrentState = WeaponStates.IDLE;
            WeaponFollowMouse();
        }
    }

    void WeaponFollowMouse()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerToMouse = (mousePosition - (Vector2)transform.position).normalized;
        animator.SetFloat("xMove", playerToMouse.x);
        animator.SetFloat("yMove", playerToMouse.y);
    }

    void WeaponFaceMovementDirection()
    {
        animator.SetFloat("xMove", playerController.moveInput.x);
        animator.SetFloat("yMove", playerController.moveInput.y);
    }
}
