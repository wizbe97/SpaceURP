using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimations : MonoBehaviour
{
    private Animator animator;
    PlayerController playerController;
    PlayerAttack playerAttack;

    public WeaponStates currentStateValue;
    public bool stateLock = false;

    public enum WeaponStates
    {
        IDLE,
        WALK,
        RUN,
        ATTACK,
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();
        playerAttack = GetComponentInParent<PlayerAttack>();
    }

    public void HandleWeaponAnimation()
    {
        if (playerAttack == null || playerController == null || gameObject.activeInHierarchy == false) 
            return;

        if (playerAttack.isAttacking)
        {
            WeaponFollowMouse();
            SetCurrentState(WeaponStates.ATTACK);
        }
        else if (playerController.moveInput != Vector2.zero && playerController.movementSpeed > 2000)
        {
            SetCurrentState(WeaponStates.RUN);
            WeaponFaceMovementDirection(playerController.moveInput);
        }
        else if (playerController.moveInput != Vector2.zero && playerController.movementSpeed <= 2000)
        {
            SetCurrentState(WeaponStates.WALK);
            WeaponFaceMovementDirection(playerController.moveInput);
        }
        else
        {
            SetCurrentState(WeaponStates.IDLE);
            if (!playerAttack.hasRecentlyAttacked)
            {
                WeaponFaceMovementDirection(playerController.lastMoveDirection);
            }
            else
            {
                WeaponFaceMovementDirection(playerAttack.GetAttackDirection());
            }
        }
    }

    private void SetCurrentState(WeaponStates newState)
    {
        currentStateValue = newState;
        switch (currentStateValue)
        {
            case WeaponStates.IDLE:
                animator.Play("Idle");
                stateLock = false;
                break;
            case WeaponStates.WALK:
                animator.Play("Walk");
                stateLock = false;
                break;
            case WeaponStates.RUN:
                animator.Play("Run");
                stateLock = false;
                break;
            case WeaponStates.ATTACK:
                animator.Play("Attack");
                stateLock = true;
                break;
        }
    }

    public void WeaponFollowMouse()
    {
        if (stateLock) return;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerToMouse = (mousePosition - (Vector2)transform.position).normalized;
        animator.SetFloat("xMove", playerToMouse.x);
        animator.SetFloat("yMove", playerToMouse.y);
    }

    public void WeaponFaceMovementDirection(Vector2 direction)
    {
        animator.SetFloat("xMove", direction.x);
        animator.SetFloat("yMove", direction.y);
    }
}
