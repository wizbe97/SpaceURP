using UnityEngine;

public class TeleportRoom : MonoBehaviour
{
    private ConfinerManager confinerManager;
    [SerializeField] private int cameraConfinerIndex;
    public Transform teleportTo;

    private void Awake()
    {
        confinerManager = FindObjectOfType<ConfinerManager>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider's child has the 'PlayerCollisions' tag
        foreach (Transform child in other.transform)
        {
            if (child.CompareTag("PlayerCollisions"))
            {
                // Teleport the parent object to the target position
                other.transform.position = teleportTo.position;
                confinerManager.UpdateConfinerCollider(cameraConfinerIndex);
                return;
            }
        }
    }
}
