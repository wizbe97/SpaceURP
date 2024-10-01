using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearAttack : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private GameObject colliderObject;
    private UpdateAnimationState animationState;
    private PlayerController playerController;
    private PlayerAttack playerAttack;

    void Start()
    {
        animator = GetComponent<Animator>();
        animationState = GetComponentInParent<UpdateAnimationState>();
        playerController = GetComponentInParent<PlayerController>();
        playerAttack = GetComponentInParent<PlayerAttack>();
    }

    public void Attack()
    {
        RotateColliderToMouse(); // Rotate the collider towards the mouse
        StartCoroutine(EnableColliderAfterDelay(0.15f));  // Start coroutine with 0.15s delay
    }

    private void RotateColliderToMouse()
    {
        // Get the mouse position in world coordinates
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // Calculate the direction from the spear to the mouse
        Vector3 direction = (mousePosition - colliderObject.transform.position).normalized;

        // Calculate the angle in radians
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the colliderObject to face the mouse position
        colliderObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private IEnumerator EnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Wait for the specified delay
        colliderObject.SetActive(true);           // Enable the collider after delay
    }

    public void AttackEnd()
    {
        colliderObject.SetActive(false);
        playerAttack.isAttacking = false;
        animationState.stateLock = false;
        animationState.UpdateCharacterAnimationState(playerController.moveInput);
        playerController.canMove = true;
    }
}
