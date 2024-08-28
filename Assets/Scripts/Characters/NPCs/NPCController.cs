using UnityEngine;

public class NPCController : MonoBehaviour
{
    public float speed = 2f;
    public float changeDirectionTime = 3f;
    public float detectionDistance = 1f;
    public LayerMask obstacleLayer;
    public int maxAttempts = 10;
    public bool canMove = true;  // Public variable to control whether the NPC can move

    public float minPauseTime = 1f;  // Minimum time the NPC will pause
    public float maxPauseTime = 3f;  // Maximum time the NPC will pause
    public float pauseChance = 0.3f; // Chance the NPC will pause instead of moving

    public bool isMoving; // Public read-only property to check if the NPC is moving

    private Vector2 direction;
    [HideInInspector] public Rigidbody2D rb;
    private float timer;
    private bool isPausing = false;
    private float pauseTimer = 0f;

    private NPCAnimationState npcAnimationState;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        npcAnimationState = GetComponent<NPCAnimationState>();
        ChooseNewDirection();
    }

    void Update()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;  // Stop movement if canMove is false
            isMoving = false;            // NPC is not moving
            return;  // Exit Update if movement is disabled
        }

        if (isPausing)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPausing = false;
                timer = 0f; // Reset the timer to immediately choose a new direction
            }
            else
            {
                rb.velocity = Vector2.zero;  // Stop movement during pause
                isMoving = false;            // NPC is not moving during pause
                return;
            }
        }

        timer += Time.deltaTime;
        if (timer >= changeDirectionTime)
        {
            ChooseNewDirection();
            timer = 0f;
        }

        // Move the NPC
        rb.velocity = direction * speed;
        isMoving = direction != Vector2.zero; // Update isMoving based on direction
        npcAnimationState.UpdateAnimationState();


        // Check for obstacles in the direction of movement
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionDistance, obstacleLayer);
        if (hit.collider != null)
        {
            ChooseNewDirection();  // Choose a new direction if an obstacle is detected
        }

    }

    void ChooseNewDirection()
    {
        if (Random.value < pauseChance)
        {
            isPausing = true;
            pauseTimer = Random.Range(minPauseTime, maxPauseTime);
            isMoving = false; // NPC is not moving during pause
            npcAnimationState.UpdateAnimationState();
            return;  // Exit without setting a new direction
        }

        int attempts = 0;
        bool validDirection = false;

        while (attempts < maxAttempts && !validDirection)
        {
            // Pick a random angle in radians
            float angle = Random.Range(0f, 2f * Mathf.PI);
            direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // Check if the chosen direction is free of obstacles
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionDistance, obstacleLayer);
            if (hit.collider == null)
            {
                validDirection = true;  // Valid direction found
                npcAnimationState.UpdateAnimationState();

            }

            attempts++;
        }

        if (!validDirection)
        {
            // If no valid direction was found after maxAttempts, stop or reverse direction
            direction = -direction; // Alternatively, you could stop the NPC by setting direction to Vector2.zero
            npcAnimationState.UpdateAnimationState();

        }

        isMoving = direction != Vector2.zero; // Update isMoving based on the final direction chosen
        npcAnimationState.UpdateAnimationState();

    }

}
