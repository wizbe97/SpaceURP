using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWeapon : MonoBehaviour
{
    private SpriteRenderer weaponSpriteRenderer;
    private Transform playerTransform;
    private int playerSortingOrder;

    [SerializeField] private float xPos = 0f;
    [SerializeField] private float yPos = 0.23f;


    [SerializeField] private bool weaponFlip = true;

    private void Start()
    {
        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = transform.parent;

        playerSortingOrder = playerTransform.GetComponent<SpriteRenderer>().sortingOrder;

        // Set initial position to the original position
        transform.localPosition = new Vector3(xPos, yPos, transform.localPosition.z);
    }

    private void Update()
    {
        RotateWeaponTowardsMouse();
        AdjustSortingOrderAndPosition();
    }

    void RotateWeaponTowardsMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - playerTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the weapon
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Flip the weapon sprite if needed
        if ((angle > 90 || angle < -90) && weaponFlip)
        {
            weaponSpriteRenderer.flipY = true;
        }
        else
        {
            weaponSpriteRenderer.flipY = false;
        }
    }

    void AdjustSortingOrderAndPosition()
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

        // Adjust X and Y positions based on angle (Z rotation)
        if (angle >= 65f && angle <= 115f)
        {
            // Set the new position (newXPos, newYPos) when angle is between 65° and 115°
            transform.localPosition = new Vector3(xPos, 0.1f, transform.localPosition.z);
        }
        else if (angle >= -117f && angle < -64f)
        {
            // Set the new position (newXPos, newYPos) when angle is between -64° and -117°
            transform.localPosition = new Vector3(-0.32f, 0.32f, transform.localPosition.z);
        }
        else if (angle >= -63 && angle < -25)
        {
            // Set the new position (newXPos, newYPos) when angle is between -25° and -63°
            transform.localPosition = new Vector3(-0.19f, 0.23f, transform.localPosition.z);
        }
        else if (angle >= -25 && angle < 25)
        {
            transform.localPosition = new Vector3(-0.14f, 0.13f, transform.localPosition.z);

        }
        else if (angle >= 116f && angle <= 153f)
        {
            transform.localPosition = new Vector3(-0.3f, 0.28f, transform.localPosition.z);
        }

        else
        {
            // Set back to the original position (originalXPos, originalYPos)
            transform.localPosition = new Vector3(xPos, yPos, transform.localPosition.z);
        }

        // Apply the adjusted sorting order to the weapon sprite renderer
        weaponSpriteRenderer.sortingOrder = sortingOrder;
    }
}
