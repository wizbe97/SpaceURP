using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalGuardian : Boss
{
    // Add a class-level variable for the fire point
    private Transform firePoint;

    // Existing variables...
    [Header("Crystal Spikes Ability")]
    [SerializeField] private GameObject crystalSpikes;
    [SerializeField] private float spawnGap = 2.0f;
    [SerializeField] private int minNumberOfSpikes = 3;
    [SerializeField] private int maxNumberOfSpikes = 10;
    [SerializeField] private float crystalSpikeDuration = 3.0f;

    [Header("Crystal Laser Ability")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform firePointDownLeft;
    [SerializeField] private Transform firePointDownRight;
    [SerializeField] private Transform firePointUpLeft;
    [SerializeField] private Transform firePointUpRight;
    private Transform[] firePoints;

    private GameObject[] crystals;
    private Transform crystal;
    [SerializeField] private GameObject startVFX;
    [SerializeField] private GameObject endVFX;
    [SerializeField] private float splitLaserSpawnDelay = 0.5f;
    [SerializeField] private float laserTime = 5.0f;

    [Header("Rock Fall Ability")]
    [SerializeField] private float rockFallTime = 5.0f;

    private readonly List<ParticleSystem> particles = new();
    private Transform player;
    private bool isSpawningSpikes = false;

    private List<GameObject> splitLasers = new();
    private List<GameObject> splitLaserStartVFX = new();
    private List<GameObject> splitLaserEndVFX = new();

    private LineRenderer tempLine;
    private GameObject tempStartVFX;
    private GameObject tempEndVFX;

    public bool isSpecial1 = false;
    public bool isSpecial2 = false;
    public bool isSpecial3 = false;

    public bool isSpecial1Active = false;
    public bool isSpecial2Active = false;
    public bool isSpecial3Active = false;

    private CrystalGuardianMovementController cgController;
    private CrystalAnimationState crystalAnimationState;

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

        firePoints = new Transform[] { firePointDownLeft, firePointDownRight, firePointUpLeft, firePointUpRight };

        cgController = GetComponent<CrystalGuardianMovementController>();
        crystalAnimationState = GetComponent<CrystalAnimationState>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void SetupAbilities()
    {
        AddAbility(SpecialAbility1);
        AddAbility(SpecialAbility2);
        AddAbility(SpecialAbility3);
    }

    private void SpecialAbility1()
    {
        if (!isSpawningSpikes && !crystalAnimationState.stateLock)
        {
            Debug.Log("CrystalGuardian uses Special Ability 1!");
            isSpecial1 = true;
            crystalAnimationState.SetEnemyDirection();
            crystalAnimationState.UpdateAnimationState();
        }
        else
        {
            Debug.Log("Spikes are already spawning, cannot start another ability.");
        }
    }

    public IEnumerator SpawnSpikes()
    {
        isSpecial1Active = true;
        DisableLaser();
        isSpawningSpikes = true;
        crystalAnimationState.stateLock = true;

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
        crystalAnimationState.PlayEndAnimation();
        ResetAbilityStates();
    }

    public IEnumerator LaserRoutine()
    {
        isSpecial3Active = true;
        EnableLaser();
        UpdateLaser();
        crystalAnimationState.stateLock = true;
        yield return new WaitForSeconds(laserTime);
        crystalAnimationState.PlayEndAnimation();
        DisableLaser();
        ResetAbilityStates();
    }

    private void SpecialAbility2()
    {
        Debug.Log("CrystalGuardian uses Special Ability 2!");

        if (!isSpawningSpikes && !crystalAnimationState.stateLock)
        {

            isSpecial2 = true;
            crystalAnimationState.SetEnemyDirection();
            crystalAnimationState.UpdateAnimationState();
        }
        else
        {
            Debug.Log("Cannot use ability while spikes are spawning, or state locked.");
        }
    }

    private void SpecialAbility3()
    {
        Debug.Log("CrystalGuardian uses Special Ability 3!");

        if (!isSpawningSpikes && !crystalAnimationState.stateLock)
        {
            isSpecial3 = true;
            crystalAnimationState.SetEnemyDirection();
            crystalAnimationState.UpdateAnimationState();
        }
        else
        {
            Debug.Log("Cannot use ability while spikes are spawning, or state locked.");
        }
    }

    public IEnumerator RockFallRoutine()
    {
        isSpecial2Active = true;
        cgController.canMove = false;
        crystalAnimationState.stateLock = true;
        yield return new WaitForSeconds(rockFallTime);
        crystalAnimationState.PlayEndAnimation();
        cgController.canMove = true;
        ResetAbilityStates();
    }

    private void EnableLaser()
    {
        crystal = GetClosestCrystal();
        firePoint = GetClosestFirePoint();

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

    public Transform Crystal
    {
        get { return crystal; }
    }

    private Transform GetClosestFirePoint()
    {
        Transform closestFirePoint = null;
        float minDistance = Mathf.Infinity;
        if (crystal != null)
        {
            foreach (Transform fp in firePoints)
            {
                float distance = Vector3.Distance(fp.position, crystal.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestFirePoint = fp;
                }
            }
        }
        return closestFirePoint;
    }

    private void DisableLaser()
    {
        if (tempLine != null)
        {
            tempLine.enabled = false;
            Destroy(tempLine.gameObject);
            Destroy(tempStartVFX?.gameObject);
            Destroy(tempEndVFX?.gameObject);
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
        if (crystal == null || firePoint == null) return;

        // Update the positions of the line renderer
        tempLine.SetPosition(0, firePoint.position);
        tempLine.SetPosition(1, crystal.position);

        // Instantiate and update VFX
        if (tempStartVFX != null)
        {
            Destroy(tempStartVFX);
        }
        if (tempEndVFX != null)
        {
            Destroy(tempEndVFX);
        }
        tempStartVFX = Instantiate(startVFX, firePoint.position, Quaternion.identity);
        tempEndVFX = Instantiate(endVFX, crystal.position, Quaternion.identity);

        // Update the direction and check for hits
        Vector2 direction = (Vector2)crystal.position - (Vector2)firePoint.position;
        float distance = direction.magnitude;
        direction.Normalize();

        int laserMask = LayerMask.GetMask("TilemapColliders", "IgnoreCrystal");
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, distance, laserMask);

        if (hit.collider != null)
        {
            Debug.Log("Laser hit: " + hit.collider.name);
            tempLine.SetPosition(1, hit.point);

            if (hit.collider.CompareTag("LaserHitTarget"))
            {
                StartCoroutine(SpawnSplitLasersWithDelay(hit.point));
            }
        }
        else
        {
            tempLine.SetPosition(1, crystal.position);
        }

        // Update the end VFX position
        if (tempEndVFX != null)
        {
            tempEndVFX.transform.position = tempLine.GetPosition(1);
        }
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
        crystalAnimationState.PlayEndAnimation();
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

    public void SetAbilityStates(bool special1, bool special2, bool special3)
    {
        isSpecial1 = special1;
        isSpecial2 = special2;
        isSpecial3 = special3;
        crystalAnimationState.UpdateAnimationState();
    }

    public void ResetAbilityStates()
    {
        crystalAnimationState.stateLock = false;
        isSpawningSpikes = false;
        DisableLaser();
        SetAbilityStates(false, false, false);
        crystalAnimationState.animator.SetBool("specialLoop", false);
        crystalAnimationState.UpdateAnimationState();
        isSpecial1Active = false;
        isSpecial2Active = false;
        isSpecial3Active = false;
    }
}
