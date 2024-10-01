using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private UpdateAnimationState animationState;
    private PlayerAttack playerAttack;
    private DialogueManager dialogueManager;
    private Action action;
    private Rigidbody2D rb;
    [SerializeField] private float moveDrag = 15f;
    [SerializeField] private float stopDrag = 25f;
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashCooldown = 3f;
    private bool dashOnCooldown = false;
    public bool isMoving = false;
    public bool canMove = true;
    [HideInInspector] public Vector2 lastMoveDirection;


    public float movementSpeed = 1250f;
    [HideInInspector] public Vector2 moveInput = Vector2.zero;
    [HideInInspector] public bool isDashing;

    private BoxCollider2D boxCollider;

    bool IsMoving
    {
        set
        {
            isMoving = value;
            animationState.UpdateCharacterAnimationState(moveInput);

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animationState = GetComponent<UpdateAnimationState>();
        action = GetComponent<Action>();
        boxCollider = GetComponent<BoxCollider2D>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void MoveCharacter()
    {
        if (canMove && moveInput != Vector2.zero)
        {
            rb.AddForce(movementSpeed * Time.fixedDeltaTime * moveInput, ForceMode2D.Force);
            lastMoveDirection = moveInput;  // Store the last movement direction
            IsMoving = true;
            playerAttack.hasRecentlyAttacked = false;
        }
        else
        {
            IsMoving = false;
        }
    }

    private void OnDash()
    {
        if (!dashOnCooldown && moveInput.magnitude > 0)
        {
            boxCollider.enabled = false;
            action.DeactivateCurrentItem();
            Vector2 dashDirection = moveInput.normalized;
            rb.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);
            isDashing = true;
            dashOnCooldown = true;
            animationState.UpdateCharacterAnimationState(moveInput);
            StartCoroutine(DashCooldown());
        }
    }

    public void OnDashEnd()
    {
        isDashing = false;
        animationState.UpdateCharacterAnimationState(moveInput);
        action.CurrentItem();
        animationState.stateLock = false;
        StartCoroutine(ReactivateColliderAfterDelay(0.5f)); // Start the coroutine with a 1-second delay
    }

    // Coroutine to reactivate the box collider after a delay
    private IEnumerator ReactivateColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        boxCollider.enabled = true;
    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        dashOnCooldown = false;
    }

    private void OnOpenInventory()
    {
        Inventory inventory = Inventory.Instance;

        GameObject backpack = inventory.transform.Find("Backpack").gameObject;

        backpack.SetActive(!backpack.activeSelf);
    }

    private void OnInteract()
    {
        dialogueManager.StartDialogue();
    }
}
