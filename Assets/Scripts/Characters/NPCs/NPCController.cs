using UnityEngine;

public class NPCController : MonoBehaviour
{
    public float moveSpeed = 10000f; // Speed for wandering
    public float wanderTime = 5f; // Time interval for changing wander direction
    public float detectionDistance = 1f; // Distance to detect obstacles
    public LayerMask obstacleLayer; // LayerMask to specify the obstacle layer

    public float moveDrag = 15f;
    public float stopDrag = 25f;

    public int maxAttempts = 10;
    public bool canMove = true;  // Public variable to control whether the NPC can move

    private Rigidbody2D rb;
    private Vector2 wanderDirection;
    private float nextWanderTime;
    private bool isPaused = false; // State to check if the NPC is currently paused
    private float nextPauseTime; // Time for the next pause
    private float pauseDuration; // Duration of the current pause

    private float nextRandomPauseTime; // When to take the next random pause
    private float randomPauseDuration; // How long the random pause will last

    private NPCAnimationState npcAnimationState;

    public bool isMoving; // Internal variable to track moving state
    public bool IsMoving
    {
        get { return isMoving; }
        set
        {
            isMoving = value;
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

    void Start()
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
                    IsMoving = true;
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
            IsMoving = false;
            return;
        }

        // Check for obstacles in the current wander direction
        if (IsObstacleDetected())
        {
            HandleCollisionPause();
            return;
        }

        // Move with the current direction
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, moveSpeed).normalized;
        rb.AddForce(moveSpeed * Time.deltaTime * wanderDirection, ForceMode2D.Force);
        IsMoving = true;
    }

    private bool IsObstacleDetected()
    {
        // Cast a ray in the direction the NPC is moving to detect obstacles on the specified layer
        RaycastHit2D hit = Physics2D.Raycast(rb.position, wanderDirection, detectionDistance, obstacleLayer);
        return hit.collider != null;
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
        // Generate a new random direction
        Vector2 newDirection = Random.insideUnitCircle.normalized;
        int attempts = 0;

        while (IsObstacleDetected() && attempts < maxAttempts)
        {
            newDirection = Random.insideUnitCircle.normalized;
            attempts++;
        }

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
        // Return a random pause interval between 3 and 10 seconds
        return Random.Range(3f, 10f);
    }

    private float GetRandomPauseLength()
    {
        // Return a random pause length between 2 and 8 seconds
        return Random.Range(2f, 8f);
    }

    private void OnDrawGizmosSelected()
    {
        // Ensure that rb is not null before accessing it
        if (rb != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(rb.position, rb.position + wanderDirection * detectionDistance);
        }
    }

}
