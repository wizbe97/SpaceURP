using UnityEngine;

public class NPCMovementController : MonoBehaviour
{
    // External Components
    [Header("External Components")]
    [Tooltip("Reference to the NPC's Rigidbody2D component.")]
    private Rigidbody2D rb;

    [Tooltip("Reference to the NPC's animation state controller.")]
    private NPCAnimationState npcAnimationState;

    // Movement Parameters
    [Header("Movement Parameters")]
    [Tooltip("Speed at which the NPC moves.")]
    public float moveSpeed = 3000f;

    [Tooltip("Drag applied when the NPC is moving.")]
    public float moveDrag = 15f;

    [Tooltip("Drag applied when the NPC stops moving.")]
    public float stopDrag = 25f;

    [Tooltip("Time interval for changing the NPC's wander direction.")]
    public float wanderTime = 5f;

    [Tooltip("Distance used to detect obstacles in the NPC's path.")]
    public float detectionDistance = 1f;

    [Tooltip("LayerMask to specify which layers are considered obstacles. Multiple layers can be selected.")]
    public LayerMask obstacleLayers;

    [Header("Raycast Settings")]
    [Tooltip("Offset for the raycast origin relative to the NPC's position.")]
    [SerializeField] private Vector2 raycastOffset = new(0, -0.5f); // Default offset downward


    // Movement Control
    [Header("Movement Control")]
    [Tooltip("Controls whether the NPC can move.")]
    public bool canMove = true;

    [Tooltip("Delay before NPC starts moving again after a conversation.")]
    public float moveAfterConversationDelay = 1.0f;

    // Pausing Parameters
    [Header("Pausing Parameters")]
    [Tooltip("Minimum time before the NPC takes the next random pause.")]
    [SerializeField] private float minTimeBeforeNextPause = 3f;

    [Tooltip("Maximum time before the NPC takes the next random pause.")]
    [SerializeField] private float maxTimeBeforeNextPause = 10f;

    [Tooltip("Minimum duration for a random pause.")]
    [SerializeField] private float minTimeToPauseFor = 2f;

    [Tooltip("Maximum duration for a random pause.")]
    [SerializeField] private float maxTimeToPauseFor = 8f;

    // Internal State
    [Header("Internal State (Do Not Modify)")]
    [Tooltip("Current direction the NPC is wandering.")]
    private Vector2 wanderDirection;

    [Tooltip("Time when the NPC will change its wander direction.")]
    private float nextWanderTime;

    [Tooltip("Indicates if the NPC is currently paused.")]
    private bool isPaused = false;

    [Tooltip("Time when the NPC will stop pausing and resume movement.")]
    private float nextPauseTime;

    [Tooltip("Duration of the current pause.")]
    private float pauseDuration;

    [Tooltip("Time for the next random pause.")]
    private float nextRandomPauseTime;

    [Tooltip("Duration of the current random pause.")]
    private float randomPauseDuration;

    [Tooltip("Tracks if the NPC is currently moving.")]
    public bool isMoving;

    public bool IsMoving
    {
        get { return isMoving; }
        set
        {
            if (canMove)
            {
                isMoving = value;
            }
            else
            {
                isMoving = false;
            }
            npcAnimationState.UpdateAnimationState();

            if (isMoving)
            {
                rb.drag = moveDrag;
            }
            else
            {
                rb.drag = stopDrag;
            }
        }
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        npcAnimationState = GetComponent<NPCAnimationState>();
        wanderDirection = Random.insideUnitCircle.normalized; // Initialize with a random direction
        nextWanderTime = Time.time + wanderTime;

        SetNextRandomPause();
    }

    void Update()
    {
        if (canMove)
        {
            if (isPaused)
            {
                if (Time.time > nextPauseTime)
                {
                    isPaused = false;
                    SetNextRandomPause();
                    nextWanderTime = Time.time + wanderTime;
                }
            }
            else
            {
                Wander();
                HandleRandomPause();
            }
        }
    }

    private void Wander()
    {
        if (Time.time > nextWanderTime)
        {
            // Switch to pausing after wandering for a while
            isPaused = true;
            pauseDuration = GetRandomPauseLength();
            nextPauseTime = Time.time + pauseDuration;
            IsMoving = false; // Stop moving when pausing
            return;
        }

        // Check for obstacles in the current wander direction
        if (IsObstacleDetectedInDirection(wanderDirection))
        {
            HandleCollisionPause();
            return;
        }

        // Move with the current direction
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, moveSpeed).normalized;
        rb.AddForce(moveSpeed * Time.deltaTime * wanderDirection, ForceMode2D.Force);
        IsMoving = true; // Set IsMoving to true only when the NPC is actively wandering
    }

    private bool IsObstacleDetectedInDirection(Vector2 direction)
    {
        // Apply the raycast offset to the NPC's position
        Vector2 raycastOrigin = (Vector2)transform.position + raycastOffset;

        // Cast the main ray in the specified direction to detect obstacles on the specified layers
        RaycastHit2D mainHit = Physics2D.Raycast(raycastOrigin, direction, detectionDistance, obstacleLayers);

        // Additional raycasts at slight angles from the main direction
        float angleOffset = 15f; // Adjust this angle for how much the side rays deviate

        Vector2 leftDirection = RotateVector(direction, -angleOffset);
        RaycastHit2D leftHit = Physics2D.Raycast(raycastOrigin, leftDirection, detectionDistance, obstacleLayers);

        Vector2 rightDirection = RotateVector(direction, angleOffset);
        RaycastHit2D rightHit = Physics2D.Raycast(raycastOrigin, rightDirection, detectionDistance, obstacleLayers);

        // Return true if any of the raycasts detect an obstacle
        return mainHit.collider != null || leftHit.collider != null || rightHit.collider != null;
    }

    private Vector2 RotateVector(Vector2 vector, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);

        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        );
    }



    private void HandleCollisionPause()
    {
        // When an obstacle is detected, pause for a random duration and find a new direction
        isPaused = true;
        pauseDuration = GetRandomPauseLength();
        nextPauseTime = Time.time + pauseDuration;
        IsMoving = false;

        FindNewWanderDirection();
    }

    private void FindNewWanderDirection()
    {
        Vector2 newDirection;

        // Continuously generate a new direction until one is found that is free of obstacles
        do
        {
            newDirection = Random.insideUnitCircle.normalized;
        }
        while (IsObstacleDetectedInDirection(newDirection));

        // Update the NPC's wander direction
        wanderDirection = newDirection;

        // Update the wander time to prevent immediate direction changes
        nextWanderTime = Time.time + wanderTime;
    }

    private void HandleRandomPause()
    {
        // Check if it's time to take a random pause
        if (Time.time > nextRandomPauseTime)
        {
            isPaused = true;
            randomPauseDuration = GetRandomPauseLength();
            nextPauseTime = Time.time + randomPauseDuration;
            IsMoving = false;
            SetNextRandomPause();
        }
    }

    private void SetNextRandomPause()
    {
        // Set when the next random pause will happen
        nextRandomPauseTime = Time.time + GetRandomPauseTime();
    }

    private float GetRandomPauseTime()
    {
        // Return a random pause interval based on the serialized min and max values
        return Random.Range(minTimeBeforeNextPause, maxTimeBeforeNextPause);
    }

    private float GetRandomPauseLength()
    {
        // Return a random pause length based on the serialized min and max values
        return Random.Range(minTimeToPauseFor, maxTimeToPauseFor);
    }

    private void OnDrawGizmos()
    {
        if (rb != null)
        {
            // Apply the raycast offset to the NPC's position
            Vector2 raycastOrigin = (Vector2)transform.position + raycastOffset;

            // Define the direction of the main ray
            Vector2 rayDirection = rb.velocity.normalized;

            // Set the gizmo color for the main ray to purple
            Gizmos.color = new Color(0.5f, 0f, 0.5f, 1f); // Purple
            Gizmos.DrawLine(raycastOrigin, raycastOrigin + rayDirection * detectionDistance);

            // Draw the additional rays with slight angles
            float angleOffset = 15f;
            Vector2 leftDirection = RotateVector(rayDirection, -angleOffset);
            Vector2 rightDirection = RotateVector(rayDirection, angleOffset);

            // Set the gizmo color for the left ray to red
            Gizmos.color = Color.red;
            Gizmos.DrawLine(raycastOrigin, raycastOrigin + leftDirection * detectionDistance);

            // Set the gizmo color for the right ray to blue
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(raycastOrigin, raycastOrigin + rightDirection * detectionDistance);
        }
    }
}

