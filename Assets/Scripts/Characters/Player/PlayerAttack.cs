using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public bool isAttacking = false;
    private Inventory inventory;
    private UpdateAnimationState animationState;
    private PlayerGun playerGun;
    private Action action;
    private SpearAttack spearAttack;

    public float lastAttackTime;
    public float spearCooldown = 1f; // Cooldown duration in seconds

    void Start()
    {
        action = GetComponent<Action>();
        inventory = FindObjectOfType<Inventory>();
        animationState = GetComponent<UpdateAnimationState>();
    }

    public void Attack()
    {
        if (animationState.stateLock == true)
            return;

        if (action.currentItem.itemType == Item.ItemType.MELEE_WEAPON)
        {
            if (Time.time - lastAttackTime < spearCooldown)
            {
                return; // Exit if cooldown is still active
            }
            isAttacking = true;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 playerToMouse = (mousePosition - (Vector2)transform.position).normalized;

            if (action.currentItem.itemName == "Spear")
            {
                spearAttack = GetComponentInChildren<SpearAttack>();
                if (spearAttack != null)
                {
                    spearAttack.Attack(playerToMouse.x, playerToMouse.y);
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
}
