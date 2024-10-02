using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapLayerAdjuster : MonoBehaviour
{
    private TilemapRenderer tilemap;
    [SerializeField] private string triggerTag = "PlayerCollisions";
    private int originalOrderInLayer;

    void Start()
    {
        // Get the TilemapRenderer component
        tilemap = GetComponent<TilemapRenderer>();

        // Store the original order in layer
        originalOrderInLayer = tilemap.sortingOrder;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object has the tag 'playercollisions'
        if (other.CompareTag(triggerTag))
        {
            // Decrease the order in layer by 1
            tilemap.sortingOrder -= 1;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the colliding object has the tag 'playercollisions'
        if (other.CompareTag(triggerTag))
        {
            // Reset the order in layer to its original value
            tilemap.sortingOrder = originalOrderInLayer;
        }
    }
}
