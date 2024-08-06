using UnityEngine;

public class CaveRooms : MonoBehaviour
{
    // Reference to the target position where the player should be teleported
    public Transform teleportTo;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider's child has the 'PlayerCollisions' tag
        foreach (Transform child in other.transform)
        {
            if (child.CompareTag("PlayerCollisions"))
            {
                // Teleport the parent object to the target position
                other.transform.position = teleportTo.position;
                return;
            }
        }
    }
}
