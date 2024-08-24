using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerGround : MonoBehaviour
{
    private int originalSortingOrder;
    private SpriteRenderer parentSpriteRenderer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCollisions"))
        {
            // Find the parent object that has the SpriteRenderer component
            parentSpriteRenderer = other.GetComponentInParent<SpriteRenderer>();

            if (parentSpriteRenderer != null)
            {
                Debug.Log("Found SpriteRenderer on: " + parentSpriteRenderer.gameObject.name);

                // Store the original sorting order
                originalSortingOrder = parentSpriteRenderer.sortingOrder;

                // Decrease the sorting order by 1
                parentSpriteRenderer.sortingOrder -= 1;

                Debug.Log("Sorting order decreased to: " + parentSpriteRenderer.sortingOrder);
            }
            else
            {
                Debug.Log("SpriteRenderer not found on parent object.");
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCollisions"))
        {
            // Ensure we have a reference to the parent's SpriteRenderer
            if (parentSpriteRenderer != null)
            {
                // Reset to the original sorting order
                parentSpriteRenderer.sortingOrder = originalSortingOrder;
                // Clear the reference to avoid potential issues
                parentSpriteRenderer = null;
            }
        }
    }
}
