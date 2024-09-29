using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    PlayerController playerController;
    private Inventory inventory;
    private PlayerGun playerGun;
    private Action action;



    public float lastAttackTime;
    private Vector2 attackDirection;

    void Start()
    {
        action = GetComponent<Action>();
        playerController = GetComponent<PlayerController>();
        inventory = FindObjectOfType<Inventory>();

    }
    public void Attack()
    {
        if (action.currentItem.itemType == Item.ItemType.MELEE_WEAPON)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            attackDirection = (mousePosition - transform.position).normalized;

            if (action.currentItem.itemName == "Spear")
            {
                // wrench.Attack(attackDirection);
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
                        // Allow shooting only when the left mouse button is pressed down
                        playerGun.Shoot();
                    }
                }
            }
        }
    }
}