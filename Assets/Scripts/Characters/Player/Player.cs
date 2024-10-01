using System.Collections;
using UnityEngine;

public class Player : Character
{
    public HitPoints hitPoints;
    public GameObject playerSpawnPoint;
    private HealthBar healthBar;
    private Inventory inventory;
    private Action action;

    private void Start()
    {
        action = GetComponent<Action>();
        SetupCharacter();
    }

    private void SetupCharacter()
    {
        healthBar = GameManager.Instance.healthBarManager;
        inventory = GameManager.Instance.inventoryManager;
        healthBar.character = this;
        hitPoints.value = startingHitPoints;
        AttachMinimapCamera();
    }

    private void AttachMinimapCamera()
    {
        // Find the "Minimap Camera" in the scene
        GameObject minimapCamera = GameObject.Find("Minimap Camera");
        if (minimapCamera != null)
        {
            // Set the camera as a child of the player
            minimapCamera.transform.SetParent(this.transform);

            // Set the camera's local position to the desired offset
            minimapCamera.transform.localPosition = new Vector3(0, 5, -20);
            minimapCamera.transform.localEulerAngles = new Vector3(15, 0, 0);

        }
        else
        {
            Debug.LogWarning("Minimap Camera not found in the scene.");
        }
    }

    public void SavePlayerData(int slot)
    {
        PlayerData playerStats = new PlayerData
        {
            health = hitPoints.value,
            position = transform.position
        };

        SaveManager.Instance.SavePlayerStats(playerStats, slot);
    }

    public void LoadPlayerData(int slot)
    {
        SetupCharacter();

        PlayerData loadedData = SaveManager.Instance.LoadPlayerStats(slot);
        if (loadedData != null)
        {
            // Apply the loaded stats to your player
            hitPoints.value = loadedData.health;
            transform.position = loadedData.position;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CanBePickedUp"))
        {
            Item hitObject = collision.gameObject.GetComponent<Consumable>().item;

            if (hitObject != null)
            {
                bool shouldDisappear = false;

                switch (hitObject.itemType)
                {
                    case Item.ItemType.HEALTH:
                        shouldDisappear = AdjustHitPoints(hitObject.quantity);
                        break;
                    case Item.ItemType.COIN:
                    case Item.ItemType.BULLET:
                    case Item.ItemType.GUN:
                    case Item.ItemType.MELEE_WEAPON:
                        inventory.CollectItem(collision.gameObject);
                        shouldDisappear = AddItemToInventory(hitObject, hitObject.quantity);
                        action.CurrentItem();
                        break;
                }

                if (shouldDisappear)
                {
                    // Notify ObjectManager when an object is destroyed
                    Destroy(collision.gameObject);
                }
            }
        }
    }

    bool AddItemToInventory(Item item, int quantity)
    {
        return inventory.AddItem(item, quantity);
    }

    public bool AdjustHitPoints(int amount)
    {
        if (hitPoints.value < maxHitPoints)
        {
            hitPoints.value = hitPoints.value + amount;
            return true;
        }
        return false;
    }

    public override IEnumerator DamageCharacter(int damage, float interval)
    {
        if (hitPoints.value <= 0)
        {
            yield break; // Exit the coroutine if the enemy is already dead
        }

        while (true)
        {
            StartCoroutine(FlickerCharacter());
            hitPoints.value = hitPoints.value - damage;

            if (hitPoints.value <= float.Epsilon)
            {
                KillCharacter();
                break;
            }

            if (interval > float.Epsilon)
            {
                yield return new WaitForSeconds(interval);
            }
            else
            {
                break;
            }
        }
    }

    public override void KillCharacter()
    {
        gameObject.SetActive(false);
        RespawnManager respawnManager = FindObjectOfType<RespawnManager>();
        if (respawnManager != null)
        {
            respawnManager.StartRespawnCoroutine(gameObject, 2f);
        }
    }


    private IEnumerator ResetCharacterAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetCharacter();
    }

    public override void ResetCharacter()
    {
        transform.position = playerSpawnPoint.transform.position;
        hitPoints.value = startingHitPoints;
        gameObject.SetActive(true);
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
