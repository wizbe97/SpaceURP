using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Action : MonoBehaviour
{
    private Inventory inventory;
    private PlayerAttack playerAttack;
    public bool isHoldingWeapon = false;
    [HideInInspector] public GameObject instantiatedCurrentItem;
    public Item currentItem;
    private bool overUI;

    void Start()
    {
        inventory = FindAnyObjectByType<Inventory>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    void Update()
    {
        overUI = IsPointerOverUI();
        if (overUI != true && Input.GetMouseButton(0)) // Check if left mouse button is held down
        {
            OnUseItem();
        }
    }

    private bool IsPointerOverUI()  // Check if the pointer is over a UI element
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
    private void OnUseItem()
    {
        if (overUI || currentItem == null)
            return;

        if (currentItem.itemType == Item.ItemType.GUN || currentItem.itemType == Item.ItemType.MELEE_WEAPON)
        {
            playerAttack.Attack();
        }
    }


    private void OnDropItem()
    {
        currentItem = inventory.GetSelectedItem(true);
        if (currentItem != null && !PlayerGun.IsAnyGunShooting())
        {

            // Calculate drop direction based on the mouse position if holding a gun
            Vector3 dropDirection;
            if (isHoldingWeapon)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                dropDirection = (mousePosition - transform.position).normalized;
            }
            else
            {
                // Otherwise, use the player's movement direction
                PlayerController playerController = GetComponent<PlayerController>();
                dropDirection = playerController.moveInput.normalized;
            }

            Vector3 dropPosition = transform.position + dropDirection * 2f;

            GameObject droppedItem = Instantiate(currentItem.droppedItem, dropPosition, Quaternion.identity);

            // Disable collider temporarily to prevent instant pickup
            if (droppedItem.TryGetComponent<Collider2D>(out var itemCollider))
            {
                itemCollider.enabled = false;
                StartCoroutine(EnableColliderAfterDelay(itemCollider));
            }

            DeactivateCurrentItem();
        }

    }

    private IEnumerator EnableColliderAfterDelay(Collider2D collider)
    {
        yield return new WaitForSeconds(1f); // Adjust the delay as needed
        collider.enabled = true;
    }

    public void CurrentItem()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogWarning("Inventory instance is null!");
            return;
        }

        currentItem = Inventory.Instance.GetSelectedItem(false);

        if (currentItem == null || !currentItem.holdable)
        {
            DeactivateCurrentItem();
            return;
        }

        if (currentItem.itemType == Item.ItemType.GUN)
        {
            isHoldingWeapon = true;

            ActivateCurrentItem();
        }
        else if (currentItem.itemType == Item.ItemType.MELEE_WEAPON)
        {
            ActivateCurrentItem();
        }
        else
        {
            DeactivateCurrentItem();
        }
    }

    public void ActivateCurrentItem()
    {
        Transform existingItemTransform = transform.Find(currentItem.itemName);
        instantiatedCurrentItem = existingItemTransform != null ? existingItemTransform.gameObject :
                              Instantiate(currentItem.instantiatedPrefab, transform.position, Quaternion.identity);
        instantiatedCurrentItem.name = currentItem.itemName;
        instantiatedCurrentItem.transform.parent = transform;
        instantiatedCurrentItem.SetActive(true);
    }
    public void DeactivateCurrentItem()
    {
        if (currentItem == null)
            return;
        
        if (instantiatedCurrentItem != null)
        {
            isHoldingWeapon = false;
            instantiatedCurrentItem.SetActive(false);
        }
        else {
            Debug.LogWarning("No item to deactivate!");
        }
    }
}