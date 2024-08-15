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
    [SerializeField] private float laserTime = 5.0f; // Duration of the laser ability

    // Rock Fall Ability Variables
    [Header("Rock Fall Ability")]
    [SerializeField] private float rockFallTime = 5.0f; // Duration of the rock fall ability

    private readonly List<ParticleSystem> particles = new();

    private Transform player; // Player's transform
    private bool isSpawningSpikes = false; // Flag to indicate if spikes are being spawned

    private List<GameObject> splitLasers = new(); // List to track split lasers
    private List<GameObject> splitLaserStartVFX = new(); // List to track start VFX of split lasers
    private List<GameObject> splitLaserEndVFX = new(); // List to track end VFX of split lasers

    private LineRenderer tempLine;
    private GameObject tempStartVFX;
    private GameObject tempEndVFX;

    // Ability state variables
    public bool isSpecial1 = false;
    public bool isSpecial2 = false;
    public bool isSpecial3 = false;

    private CrystalGuardianMovementController cgController;
    private CrystalAnimationState crystalAnimationState;

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
        cgController = GetComponent<CrystalGuardianMovementController>();
        crystalAnimationState = GetComponent<CrystalAnimationState>();
    }

    protected override void Update()
    {
        base.Update();
    }

    // Setup abilities specific to CrystalGuardian
    protected override void SetupAbilities()
    {
        AddAbility(SpecialAbility1);
        AddAbility(SpecialAbility2);
        AddAbility(SpecialAbility3);
    }

    // Special Ability 1: Crystal Spikes
    private void SpecialAbility1()
    {
        // Ensure other abilities are disabled

        // Check if spikes are already spawning
        if (!isSpawningSpikes && !crystalAnimationState.stateLock)
        {
            Debug.Log("CrystalGuardian uses Special Ability 1!");
            StartCoroutine(SpawnSpikes());
            SetAbilityStates(true, false, false);
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
        isSpawningSpikes = true;

        int numberOfSpikes = Random.Range(minNumberOfSpikes, maxNumberOfSpikes + 1);
        for (int i = 0; i < numberOfSpikes; i++)
        {
            if (crystalSpikes != null && player != null)
            {
                GameObject spawnedSpike = Instantiate(crystalSpikes, player.position, Quaternion.identity);
                Destroy(spawnedSpike, crystalSpikeDuration);
            }
            yield return new WaitForSeconds(spawnGap);
        }

        isSpawningSpikes = false;
        ResetAbilityStates();
    }

    private IEnumerator LaserRoutine()
    {
        EnableLaser();
        UpdateLaser();
        yield return new WaitForSeconds(laserTime);
        DisableLaser();
        ResetAbilityStates();
    }

    // Special Ability 2: Rock Fall
    private void SpecialAbility2()
    {
        Debug.Log("CrystalGuardian uses Special Ability 3!");


        if (!isSpawningSpikes && !crystalAnimationState.stateLock)
        {
            StartCoroutine(RockFallRoutine());
            SetAbilityStates(false, true, false);

        }
        else
        {
            Debug.Log("Cannot use ability while spikes are spawning, or state locked.");
        }
    }

    // Special Ability 3: Crystal Laser
    private void SpecialAbility3()
    {
        Debug.Log("CrystalGuardian uses Special Ability 2!");

        if (!isSpawningSpikes && !crystalAnimationState.stateLock)
        {
            StartCoroutine(LaserRoutine());
            SetAbilityStates(false, false, true);
        }
        else
        {
            Debug.Log("Cannot use ability while spikes are spawning, or state locked.");
        }
    }

    private IEnumerator RockFallRoutine()
    {
        cgController.canMove = false;
        yield return new WaitForSeconds(rockFallTime);
        cgController.canMove = true;
        ResetAbilityStates();
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

        foreach (GameObject laser in splitLasers)
        {
            if (laser != null)
            {
                Destroy(laser);
            }
        }
        splitLasers.Clear();

        foreach (GameObject vfx in splitLaserStartVFX)
        {
            if (vfx != null)
            {
                Destroy(vfx);
            }
        }
        splitLaserStartVFX.Clear();

        foreach (GameObject vfx in splitLaserEndVFX)
        {
            if (vfx != null)
            {
                Destroy(vfx);
            }
        }
        splitLaserEndVFX.Clear();
    }

    private void UpdateLaser()
    {
        tempLine.SetPosition(0, firePoint.position);
        tempLine.SetPosition(1, crystal.position);

        tempStartVFX = Instantiate(startVFX, tempLine.GetPosition(0), Quaternion.identity);
        tempEndVFX = Instantiate(endVFX, tempLine.GetPosition(1), Quaternion.identity);

        startVFX.transform.position = firePoint.position;

        Vector2 direction = (Vector2)crystal.position - (Vector2)firePoint.position;
        float distance = direction.magnitude;
        direction.Normalize();

        int laserMask = LayerMask.GetMask("TilemapColliders", "IgnoreCrystal");
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, distance, laserMask);

        if (hit.collider != null)
        {
            Debug.Log("Laser hit: " + hit.collider.name);
            tempLine.SetPosition(1, hit.point);

            if (hit.collider.transform.tag == "LaserHitTarget")
            {
                StartCoroutine(SpawnSplitLasersWithDelay(hit.point));
            }
        }
        else
        {
            tempLine.SetPosition(1, crystal.position);
        }

        endVFX.transform.position = tempLine.GetPosition(1);
    }

    private IEnumerator SpawnSplitLasersWithDelay(Vector2 spawnPosition)
    {
        yield return new WaitForSeconds(splitLaserSpawnDelay);
        SpawnSplitLasers(spawnPosition);
    }

    private void SpawnSplitLasers(Vector2 spawnPosition)
    {
        Vector2[] directions = new Vector2[]
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right,
            new Vector2(1, 1).normalized,
            new Vector2(-1, 1).normalized,
            new Vector2(1, -1).normalized,
            new Vector2(-1, -1).normalized
        };

        int originalLayer = crystal.gameObject.layer;
        int ignoreLayer = LayerMask.NameToLayer("IgnoreCrystal");
        crystal.gameObject.layer = ignoreLayer;

        foreach (Vector2 dir in directions)
        {
            GameObject newLaser = new GameObject("SplitLaser");
            LineRenderer newLaserLineRenderer = newLaser.AddComponent<LineRenderer>();

            newLaserLineRenderer.material = tempLine.sharedMaterial;
            newLaserLineRenderer.startWidth = tempLine.startWidth;
            newLaserLineRenderer.endWidth = tempLine.endWidth;
            newLaserLineRenderer.startColor = tempLine.startColor;
            newLaserLineRenderer.endColor = tempLine.endColor;
            newLaserLineRenderer.sortingLayerName = tempLine.sortingLayerName;
            newLaserLineRenderer.sortingOrder = tempLine.sortingOrder;

            newLaserLineRenderer.SetPosition(0, spawnPosition);

            int tilemapLayerMask = LayerMask.GetMask("TilemapColliders");
            RaycastHit2D hit = Physics2D.Raycast(spawnPosition, dir, Mathf.Infinity, tilemapLayerMask);
            if (hit.collider != null)
            {
                newLaserLineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                newLaserLineRenderer.SetPosition(1, spawnPosition + dir * 10.0f);
            }
            newLaser.AddComponent<CrystalLaser>();

            GameObject startVFXInstance = Instantiate(startVFX, spawnPosition, Quaternion.identity);
            splitLaserStartVFX.Add(startVFXInstance);

            GameObject endVFXInstance = Instantiate(endVFX, newLaserLineRenderer.GetPosition(1), Quaternion.identity);
            splitLaserEndVFX.Add(endVFXInstance);

            splitLasers.Add(newLaser);
        }

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

    private void SetAbilityStates(bool special1, bool special2, bool special3)
    {
        isSpecial1 = special1;
        isSpecial2 = special2;
        isSpecial3 = special3;
        crystalAnimationState.UpdateAnimationState();
    }

    private void ResetAbilityStates()
    {
        SetAbilityStates(false, false, false);
        crystalAnimationState.UpdateAnimationState();
    }
}