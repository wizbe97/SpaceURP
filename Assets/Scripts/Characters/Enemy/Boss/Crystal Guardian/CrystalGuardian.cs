using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalGuardian : Boss
{
    // Crystal Spikes Ability Variables
    [Header("Crystal Spikes Ability")]
    [SerializeField] private GameObject crystalSpikes; // Spike prefab
    [SerializeField] private float spawnGap = 2.0f; // Gap time between spawns
    [SerializeField] private int minNumberOfSpikes = 3; // Minimum number of spikes to spawn
    [SerializeField] private int maxNumberOfSpikes = 10; // Maximum number of spikes to spawn
    [SerializeField] private float crystalSpikeDuration = 3.0f; // Duration before crystal spikes are destroyed

    // Crystal Laser Ability Variables
    [Header("Crystal Laser Ability")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform firePoint;
    private GameObject[] crystals;
    private Transform crystal;
    [SerializeField] private GameObject startVFX;
    [SerializeField] private GameObject endVFX;
    [SerializeField] private float splitLaserSpawnDelay = 0.5f; // Delay before split lasers spawn

    private readonly List<ParticleSystem> particles = new();

    private Transform player; // Player's transform
    private bool isSpawningSpikes = false; // Flag to indicate if spikes are being spawned

    private List<GameObject> splitLasers = new(); // List to track split lasers
    private List<GameObject> splitLaserStartVFX = new(); // List to track start VFX of split lasers
    private List<GameObject> splitLaserEndVFX = new(); // List to track end VFX of split lasers

    private LineRenderer tempLine;
    private GameObject tempStartVFX;
    private GameObject tempEndVFX;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        crystals = GameObject.FindGameObjectsWithTag("LaserHitTarget");
        FillLists();
        DisableLaser();

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player GameObject not found. Please ensure it is tagged 'Player'.");
        }
    }

    protected override void Update()
    {
        base.Update();

        if(Input.GetKeyDown(KeyCode.L))
        {
            SpecialAbility2();
        }
    }
    // Setup abilities specific to CrystalGuardian
    protected override void SetupAbilities()
    {
        AddAbility(SpecialAbility1);
        AddAbility(SpecialAbility2);
        AddAbility(SpecialAbility3);
    }

    // Example ability 1
    private void SpecialAbility1()
    {
        Debug.Log("CrystalGuardian uses Special Ability 1!");

        // Check if spikes are already spawning
        if (!isSpawningSpikes)
        {
            // Start the coroutine to spawn spikes underneath the player
            StartCoroutine(SpawnSpikes());
        }
        else
        {
            Debug.Log("Spikes are already spawning, cannot start another ability.");
        }
    }

    // Coroutine to spawn spikes with a gap in between
    private IEnumerator SpawnSpikes()
    {
        DisableLaser();
        // Set the flag to true to indicate spikes are spawning
        isSpawningSpikes = true;

        // Determine a random number of spikes to spawn within the specified range
        int numberOfSpikes = Random.Range(minNumberOfSpikes, maxNumberOfSpikes + 1); // +1 because max range is exclusive

        for (int i = 0; i < numberOfSpikes; i++)
        {
            if (crystalSpikes != null && player != null)
            {
                // Spawn spike at player's current position
                GameObject spawnedSpike = Instantiate(crystalSpikes, player.position, Quaternion.identity);
                // Destroy the spike after the specified duration
                Destroy(spawnedSpike, crystalSpikeDuration);
            }

            // Wait for the specified gap time before spawning the next spike
            yield return new WaitForSeconds(spawnGap);
        }

        // Set the flag to false to indicate spikes have finished spawning
        isSpawningSpikes = false;
    }

    // Example ability 2
    private void SpecialAbility2()
    {
        Debug.Log("CrystalGuardian uses Special Ability 2!");

        // Prevent this ability from starting if spikes are spawning
        if (!isSpawningSpikes)
        {
            DisableLaser();
            EnableLaser();
            UpdateLaser();
        }
        else
        {
            Debug.Log("Cannot use ability while spikes are spawning.");
        }
    }

    // Example ability 3
    private void SpecialAbility3()
    {
        Debug.Log("CrystalGuardian uses Special Ability 3!");

        // Prevent this ability from starting if spikes are spawning
        if (!isSpawningSpikes)
        {
            DisableLaser();
            EnableLaser();
            UpdateLaser();
        }
        else
        {
            Debug.Log("Cannot use ability while spikes are spawning.");
        }
    }

    // Enable the laser ability
    private void EnableLaser()
    {

        crystal = GetClosestCrystal();

        if (tempLine == null)
        {
            tempLine = Instantiate(lineRenderer);
        }
        if (tempLine != null)
        {
            tempLine.enabled = true;
        }
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Play();
        }
    }

    Transform GetClosestCrystal()
    {
        Transform closestCrystal = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject gb in crystals)
        {
            float distance = Vector3.Distance(gb.transform.position, currentPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestCrystal = gb.transform;
            }
        }

        return closestCrystal;
    }
    // Disable the laser ability
    private void DisableLaser()
    {
        // Check if the laser ability is already disabled
        if (tempLine != null)
        {
            tempLine.enabled = false;
            Destroy(tempLine.gameObject);
            Destroy(tempStartVFX.gameObject);
            Destroy(tempEndVFX.gameObject);
            tempLine = null;
            tempStartVFX = null;
            tempEndVFX = null;
        }
        else
        {
            Debug.Log("Laser ability is already disabled.");
        }

        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Stop();
        }

        // Destroy all split lasers and their VFX
        foreach (GameObject laser in splitLasers)
        {
            if (laser != null)
            {
                Destroy(laser); // Destroy or disable the laser
            }
        }
        splitLasers.Clear(); // Clear the list

        foreach (GameObject vfx in splitLaserStartVFX)
        {
            if (vfx != null)
            {
                Destroy(vfx); // Destroy the start VFX
            }
        }
        splitLaserStartVFX.Clear(); // Clear the list

        foreach (GameObject vfx in splitLaserEndVFX)
        {
            if (vfx != null)
            {
                Destroy(vfx); // Destroy the end VFX
            }
        }
        splitLaserEndVFX.Clear(); // Clear the list
    }

    private void UpdateLaser()
    {
        tempLine.SetPosition(0, firePoint.position);  // Set the start of the laser at the fire point
        tempLine.SetPosition(1, crystal.position);    // Initially set the end at the crystal

        tempStartVFX = Instantiate(startVFX, tempLine.GetPosition(0), Quaternion.identity);
        tempEndVFX = Instantiate(endVFX, tempLine.GetPosition(1), Quaternion.identity);

        startVFX.transform.position = firePoint.position;

        Vector2 direction = (Vector2)crystal.position - (Vector2)firePoint.position;  // Calculate the direction vector
        float distance = direction.magnitude;  // Calculate the distance to the crystal
        direction.Normalize();  // Normalize the direction vector

        // Use a layer mask that includes the crystal layer
        int laserMask = LayerMask.GetMask("TilemapColliders", "IgnoreCrystal");
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, distance, laserMask);

        if (hit.collider != null)
        {
            Debug.Log("Laser hit: " + hit.collider.name);
            tempLine.SetPosition(1, hit.point);

            // Check if the laser hit the crystal
            if (hit.collider.transform.tag == "LaserHitTarget")
            {
                // Start the coroutine to spawn split lasers with a delay
                StartCoroutine(SpawnSplitLasersWithDelay(hit.point));
            }
        }
        else
        {
            // If nothing is hit, ensure the laser goes all the way to the crystal
            tempLine.SetPosition(1, crystal.position);
        }

        endVFX.transform.position = tempLine.GetPosition(1);
    }

    private IEnumerator SpawnSplitLasersWithDelay(Vector2 spawnPosition)
    {
        // Wait for the specified delay before spawning split lasers
        yield return new WaitForSeconds(splitLaserSpawnDelay);

        // Proceed to spawn split lasers
        SpawnSplitLasers(spawnPosition);
    }

    private void SpawnSplitLasers(Vector2 spawnPosition)
    {
        // Define 8 directions for the split lasers (45 degrees apart)
        Vector2[] directions = new Vector2[]
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right,
            new Vector2(1, 1).normalized,   // Top-right
            new Vector2(-1, 1).normalized,  // Top-left
            new Vector2(1, -1).normalized,  // Bottom-right
            new Vector2(-1, -1).normalized  // Bottom-left
        };

        // Save original layer and set the crystal layer to "IgnoreCrystal"
        int originalLayer = crystal.gameObject.layer;
        int ignoreLayer = LayerMask.NameToLayer("IgnoreCrystal");
        crystal.gameObject.layer = ignoreLayer;

        foreach (Vector2 dir in directions)
        {
            // Instantiate a new laser object
            GameObject newLaser = new GameObject("SplitLaser");
            LineRenderer newLaserLineRenderer = newLaser.AddComponent<LineRenderer>();

            // Copy settings from the original LineRenderer
            newLaserLineRenderer.material = tempLine.sharedMaterial;
            newLaserLineRenderer.startWidth = tempLine.startWidth;
            newLaserLineRenderer.endWidth = tempLine.endWidth;
            newLaserLineRenderer.startColor = tempLine.startColor;
            newLaserLineRenderer.endColor = tempLine.endColor;

            // Set the sorting layer and order explicitly
            newLaserLineRenderer.sortingLayerName = tempLine.sortingLayerName;
            newLaserLineRenderer.sortingOrder = tempLine.sortingOrder;

            // Set the start and end positions for the new laser
            newLaserLineRenderer.SetPosition(0, spawnPosition);

            // Raycast to determine the end position of the new laser, ignoring the crystal layer
            int tilemapLayerMask = LayerMask.GetMask("TilemapColliders");
            RaycastHit2D hit = Physics2D.Raycast(spawnPosition, dir, Mathf.Infinity, tilemapLayerMask);
            if (hit.collider != null)
            {
                newLaserLineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                // If nothing is hit, project the laser in the direction indefinitely
                newLaserLineRenderer.SetPosition(1, spawnPosition + dir * 10.0f); // Max distance of 10 units
            }
            newLaser.AddComponent<CrystalLaser>();

            // Instantiate visual effects at the start and end of the new laser
            GameObject startVFXInstance = Instantiate(startVFX, spawnPosition, Quaternion.identity);
            splitLaserStartVFX.Add(startVFXInstance);

            GameObject endVFXInstance = Instantiate(endVFX, newLaserLineRenderer.GetPosition(1), Quaternion.identity);
            splitLaserEndVFX.Add(endVFXInstance);

            // Add the new laser to the list for tracking
            splitLasers.Add(newLaser);
        }

        // Restore the crystal's original layer
        crystal.gameObject.layer = originalLayer;
    }

    void FillLists()
    {
        for (int i = 0; i < startVFX.transform.childCount; i++)
        {
            if (startVFX.transform.GetChild(i).TryGetComponent<ParticleSystem>(out var ps))
            {
                particles.Add(ps);
            }
        }

        for (int i = 0; i < endVFX.transform.childCount; i++)
        {
            if (endVFX.transform.GetChild(i).TryGetComponent<ParticleSystem>(out var ps))
            {
                particles.Add(ps);
            }
        }
    }
}
