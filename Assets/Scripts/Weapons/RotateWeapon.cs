using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWeapon : MonoBehaviour
{
    private SpriteRenderer weaponSpriteRenderer;
    private Transform playerTransform;
    private int playerSortingOrder;

    private void Start()
    {
        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = transform.parent;

        playerSortingOrder = playerTransform.GetComponent<SpriteRenderer>().sortingOrder;

        // Adjust the Y position of the weapon by 2 units
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.23f, transform.localPosition.z);
    }

    private void Update()
    {
        RotateGunTowardsMouse();
        AdjustSortingOrder();
    }
    void RotateGunTowardsMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - playerTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the weapon
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (angle > 90 || angle < -90)
        {
            weaponSpriteRenderer.flipY = true;
        }
        else
        {
            weaponSpriteRenderer.flipY = false;
        }
    }

    void AdjustSortingOrder()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - playerTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        int sortingOrder = playerSortingOrder; // Start with player's sorting order

        // Adjust sorting order based on angle
        switch (Mathf.RoundToInt(angle / 45f))
        {
            case 1: // 45 degrees
            case 2: // 90 degrees
            case 3: // 135 degrees
                sortingOrder -= 1;
                break;
            default:
                sortingOrder += 1;
                break;
        }

        // Apply the adjusted sorting order to the gun sprite renderer
        weaponSpriteRenderer.sortingOrder = sortingOrder;
    }
}
