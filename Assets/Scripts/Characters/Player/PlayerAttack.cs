using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public bool isAttacking = false;
    private Inventory inventory;
    private UpdateAnimationState animationState;
    private PlayerController playerController;
    private PlayerGun playerGun;
    private Action action;
    private SpearAttack spearAttack;

    public float lastAttackTime;
    public float spearCooldown = 1f; // Cooldown duration in seconds

    // Store the attack direction and whether the player has recently attacked
    private Vector2 attackDirection;
    public bool hasRecentlyAttacked = false; // New flag for tracking recent attack

    void Start()
    {
        action = GetComponent<Action>();
        inventory = FindObjectOfType<Inventory>();
        animationState = GetComponent<UpdateAnimationState>();
        playerController = GetComponent<PlayerController>();
    }

    public void Attack()
    {
        if (animationState.stateLock == true || playerController.isDashing == true || FindAnyObjectByType<NPCDialogue>().dialogueInitiated == true)
            return;

        if (action.currentItem.itemType == Item.ItemType.MELEE_WEAPON)
        {
            if (Time.time - lastAttackTime < spearCooldown)
                return; // Exit if cooldown is still active

            isAttacking = true;
            hasRecentlyAttacked = true; // Mark that the player has recently attacked

            // Store the attack direction (based on the mouse position)
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            attackDirection = (mousePosition - (Vector2)transform.position).normalized;

            if (action.currentItem.itemName == "Spear")
            {
                spearAttack = GetComponentInChildren<SpearAttack>();
                if (spearAttack != null)
                {
                    spearAttack.Attack();
                    lastAttackTime = Time.time; // Update the last attack time
                }
            }
        }

        if (action.currentItem.itemType == Item.ItemType.GUN)
        {
            if (action.currentItem.bullet != null)
            {
                // Check if the inventory has the required bullet type for this gun
                if (inventory.HasItem(action.currentItem.bullet))
                {
                    playerGun = GetComponentInChildren<PlayerGun>();
                    if (playerGun != null && !PlayerGun.IsAnyGunShooting())
                    {
                        playerGun.Shoot();
                    }
                }
            }
        }
    }

    public Vector2 GetAttackDirection()
    {
        return attackDirection;
    }


    public void OnAttackEnd()
    {
        isAttacking = false;
        animationState.stateLock = false;
        playerController.canMove = true;
        animationState.UpdateCharacterAnimationState(GetComponent<PlayerController>().moveInput);
    }
}
