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
        ATTACK,
        RUN
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
                case WeaponStates.RUN:
                    animator.Play("Run");
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
        if (playerAttack.isAttacking)
        {
            CurrentState = WeaponStates.ATTACK;
            WeaponFollowMouse();
        }
        else if (playerController.moveInput != Vector2.zero && playerController.movementSpeed >= 2000)
        {
            // Update the last move direction when the player is moving
            CurrentState = WeaponStates.RUN;
            WeaponFaceMovementDirection(playerController.moveInput);
        }
        else
        {
            // When idling, use the last move direction to keep the weapon facing the correct direction
            CurrentState = WeaponStates.IDLE;
            if (playerAttack.hasRecentlyAttacked == false)
                WeaponFaceMovementDirection(playerController.lastMoveDirection);
            else
                WeaponFaceMovementDirection(playerAttack.GetAttackDirection());
        }
    }


    public void WeaponFollowMouse()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerToMouse = (mousePosition - (Vector2)transform.position).normalized;
        animator.SetFloat("xMove", playerToMouse.x);
        animator.SetFloat("yMove", playerToMouse.y);
    }

    void WeaponFaceMovementDirection(Vector2 direction)
    {
        animator.SetFloat("xMove", direction.x);
        animator.SetFloat("yMove", direction.y);
    }

}
