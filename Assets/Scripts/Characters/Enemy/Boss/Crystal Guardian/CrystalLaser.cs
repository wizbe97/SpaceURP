using UnityEngine;

public class CrystalLaser : MonoBehaviour
{
    public LineRenderer lineRenderer;

    private PolygonCollider2D polyCollider;
    private int laserDamage = 10;

    private void Start()
    {
        // Initialize the collider
        InitializeCollider();
    }

    private void InitializeCollider()
    {
        // Check if the collider already exists
        polyCollider = GetComponent<PolygonCollider2D>();
        if (polyCollider == null)
        {
            // Add a new PolygonCollider2D if not already present
            polyCollider = gameObject.AddComponent<PolygonCollider2D>();
            polyCollider.isTrigger = true; // Set the collider as a trigger
        }

        UpdateCollider();
    }

    public void UpdateCollider()
    {
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned.");
            return;
        }

        // Get the start and end positions of the laser from the LineRenderer
        Vector2 start = lineRenderer.GetPosition(0);
        Vector2 end = lineRenderer.GetPosition(1);

        // Define the width of the collider
        float width = lineRenderer.startWidth;  // Or any fixed width if you want

        // Calculate the direction and perpendicular offsets
        Vector2 direction = (end - start).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x) * width;

        // Define the polygon points
        Vector2[] points = new Vector2[]
        {
            start + perpendicular,
            start - perpendicular,
            end - perpendicular,
            end + perpendicular
        };

        // Update the collider's points
        polyCollider.points = points;
    }

    // Call this method to update the collider when the line renderer is updated
    public void RefreshCollider()
    {
        UpdateCollider();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            StartCoroutine(player.DamageCharacter(laserDamage, 0)); // Set interval to 0 to ensure it's only called once
        }
    }

}
