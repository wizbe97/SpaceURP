using UnityEngine;
using UnityEngine.Rendering;

public class PlayerRenderer : MonoBehaviour
{
    public bool bringToFront = true; // Determines if the player should be rendered in front or behind
    public string triggerTag = "PlayerCollisions"; // Allows selection between PlayerCollisions or Player tag

    private int originalSortingOrder;
    private int originalSortingGroupOrder;
    private SpriteRenderer parentSpriteRenderer;
    private SortingGroup parentSortingGroup;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(triggerTag))
        {
            // Find the parent object that has the SpriteRenderer component
            parentSpriteRenderer = other.GetComponentInParent<SpriteRenderer>();
            // Find the parent object that has the SortingGroup component
            parentSortingGroup = other.GetComponentInParent<SortingGroup>();

            int orderChange = bringToFront ? 1 : -1; // Determine whether to increase or decrease sorting order

            if (parentSpriteRenderer != null)
            {
                // Store the original sorting order
                originalSortingOrder = parentSpriteRenderer.sortingOrder;

                // Change the sorting order based on the bringToFront variable
                parentSpriteRenderer.sortingOrder += orderChange;

                Debug.Log($"SpriteRenderer sorting order changed to: {parentSpriteRenderer.sortingOrder}");
            }
            else
            {
                Debug.Log("SpriteRenderer not found on parent object.");
            }

            if (parentSortingGroup != null)
            {
                // Store the original sorting order
                originalSortingGroupOrder = parentSortingGroup.sortingOrder;

                // Change the sorting order based on the bringToFront variable
                parentSortingGroup.sortingOrder += orderChange;

                Debug.Log($"SortingGroup sorting order changed to: {parentSortingGroup.sortingOrder}");
            }
            else
            {
                Debug.Log("SortingGroup not found on parent object.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(triggerTag))
        {
            // Ensure we have a reference to the parent's SpriteRenderer
            if (parentSpriteRenderer != null)
            {
                // Reset to the original sorting order
                parentSpriteRenderer.sortingOrder = originalSortingOrder;
                Debug.Log($"SpriteRenderer sorting order reset to: {originalSortingOrder}");
                // Clear the reference to avoid potential issues
                parentSpriteRenderer = null;
            }

            // Ensure we have a reference to the parent's SortingGroup
            if (parentSortingGroup != null)
            {
                // Reset to the original sorting order
                parentSortingGroup.sortingOrder = originalSortingGroupOrder;
                Debug.Log($"SortingGroup sorting order reset to: {originalSortingGroupOrder}");
                // Clear the reference to avoid potential issues
                parentSortingGroup = null;
            }
        }
    }
}
