using UnityEngine;

public class CrystalLaser : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private PolygonCollider2D polygonCollider;
    [SerializeField] private int laserDamage = 10;

    private void Awake()
    {
        // Try to get the LineRenderer component; if not found, add one
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Try to get the PolygonCollider2D component; if not found, add one
        polygonCollider = GetComponent<PolygonCollider2D>();
        if (polygonCollider == null)
        {
            polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
        }

        // Set the collider to be a trigger
        polygonCollider.isTrigger = true;
    }

    private void Start()
    {
        UpdateCollider();
    }

    private void Update()
    {
        UpdateCollider();
    }

    private void UpdateCollider()
    {
        // Ensure the line renderer has at least two points (start and end)
        if (lineRenderer.positionCount < 2)
        {
            return;
        }

        // Get the start and end positions of the laser from the LineRenderer
        Vector2 start = lineRenderer.GetPosition(0);
        Vector2 end = lineRenderer.GetPosition(1);

        // Get the current width of the laser from the LineRenderer
        float laserWidth = lineRenderer.startWidth;

        // Calculate the direction of the laser and the normal to that direction
        Vector2 direction = (end - start).normalized;
        Vector2 normal = new Vector2(-direction.y, direction.x);

        // Define the four points of the rectangle that will represent the laser's hitbox
        Vector2[] points = new Vector2[4];
        points[0] = start + normal * laserWidth / 2f;
        points[1] = start - normal * laserWidth / 2f;
        points[2] = end - normal * laserWidth / 2f;
        points[3] = end + normal * laserWidth / 2f;

        // Update the PolygonCollider2D to match the laser's current shape
        polygonCollider.SetPath(0, points);
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
