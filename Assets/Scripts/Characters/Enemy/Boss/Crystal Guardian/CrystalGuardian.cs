using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CrystalGuardian : Boss
{
    // Existing variables...
    [Header("Crystal Spikes Ability")]
    [SerializeField] private GameObject crystalSpikes;
    [SerializeField] private float crystalSpikeSpawnGap = 2.0f;
    [SerializeField] private int minNumberOfSpikes = 3;
    [SerializeField] private int maxNumberOfSpikes = 10;
    [SerializeField] private float crystalSpikeDuration = 3.0f;

    [Header("Crystal Laser Ability")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform firePointLaser;

    private GameObject[] crystals;
    private Transform crystal;
    [SerializeField] private GameObject startVFX;
    [SerializeField] private GameObject endVFX;
    [SerializeField] private float splitLaserSpawnDelay = 0.5f;
    [SerializeField] private float laserTime = 5.0f;

    [Header("Stalactite Fall Ability")]
    [SerializeField] private GameObject stalactitePrefab;
    [SerializeField] private GameObject shadowPrefab;
    [SerializeField] private float stalactiteSpawnGap = 2.0f;

    [SerializeField] private int minNumberOfstalactites = 3;
    [SerializeField] private int maxNumberOfstalactites = 10;
    [SerializeField] private float stalactiteDuration = 3.0f;
    [SerializeField] private float fallSpeed = 2.0f;



    private readonly List<ParticleSystem> particles = new();
    private Transform player;
    private bool isSpawningSpikes = false;

    private List<GameObject> splitLasers = new();
    private List<GameObject> splitLaserStartVFX = new();
    private List<GameObject> splitLaserEndVFX = new();

    private LineRenderer tempLine;
    private GameObject tempStartVFX;
    private GameObject tempEndVFX;

    [HideInInspector] public bool isSpecial1 = false;
    [HideInInspector] public bool isSpecial2 = false;
    [HideInInspector] public bool isSpecial3 = false;

    [HideInInspector] public bool isSpecial1Active = false;
    [HideInInspector] public bool isSpecial2Active = false;
    [HideInInspector] public bool isSpecial3Active = false;

    private CrystalGuardianMovementController cgController;
    private CrystalAnimationState crystalAnimationState;
    private CrystalGuardianStalactite crystalGuardianStalactite;

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
        crystalGuardianStalactite = GetComponent<CrystalGuardianStalactite>();
    }

    protected override void Update()
    {
        if (isSpecial1 || isSpecial2 || isSpecial3) return;

        base.Update();
    }

    protected override void SetupAbilities()
    {
        // AddAbility(SpecialAbility1);
        AddAbility(SpecialAbility2);
        // AddAbility(SpecialAbility3);
    }

    private void SpecialAbility1()
    {
        Debug.Log("CrystalGuardian uses Special Ability 1!");
        StartCoroutine(SpecialAbilityWaitCO(ExecuteAbility1));
    }
    private void ExecuteAbility1()
    {
        isSpecial1 = true;
        crystalAnimationState.SetEnemyDirection();
        crystalAnimationState.UpdateAnimationState();
        Debug.Log("USED Special Ability 1!");
    }
    private void SpecialAbility2()
    {
        Debug.Log("CrystalGuardian uses Special Ability 2!");
        StartCoroutine(SpecialAbilityWaitCO(ExecuteAbility2));
    }
    private void ExecuteAbility2()
    {
        isSpecial2 = true;
        crystalAnimationState.SetEnemyDirection();
        crystalAnimationState.UpdateAnimationState();
        Debug.Log("USED Special Ability 2!");
    }

    private void SpecialAbility3()
    {
        Debug.Log("CrystalGuardian uses Special Ability 3!");
        StartCoroutine(SpecialAbilityWaitCO(ExecuteAbility3));
    }
    private void ExecuteAbility3()
    {
        isSpecial3 = true;
        crystal = GetClosestCrystal();
        crystalAnimationState.SetEnemyDirection();
        crystalAnimationState.UpdateAnimationState();
        Debug.Log("USED Special Ability 3!");
    }

    private IEnumerator SpecialAbilityWaitCO(UnityAction callback)
    {
        while (isSpawningSpikes || crystalAnimationState.stateLock)
        {
            Debug.Log("Waiting for spikes to go away or state to unlock");
            yield return null;
        }

        //ADD DELAY BEFORE AFTER ATTACK FINISHES
        yield return new WaitForSeconds(0.5f);

        callback?.Invoke();
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
            yield return new WaitForSeconds(crystalSpikeSpawnGap);
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



    public IEnumerator RockFallRoutine()
    {
        isSpecial2Active = true;
        cgController.canMove = false;
        crystalAnimationState.stateLock = true;

        int numberOfstalactites = Random.Range(minNumberOfstalactites, maxNumberOfstalactites + 1);

        // This list will hold references to the instantiated shadows and stalactites
        List<GameObject> shadows = new List<GameObject>();
        List<GameObject> stalactites = new List<GameObject>();

        for (int i = 0; i < numberOfstalactites; i++)
        {
            if (stalactitePrefab != null && player != null)
            {
                // Spawn the shadow at the player's position
                GameObject spawnedShadow = Instantiate(shadowPrefab, player.position, Quaternion.identity);
                shadows.Add(spawnedShadow);

                // Spawn the stalactite off-screen (above the play area)
                Vector3 offScreenPosition = new Vector3(player.position.x, player.position.y + 15.0f, player.position.z);
                GameObject spawnedStalactite = Instantiate(stalactitePrefab, offScreenPosition, Quaternion.identity);
                stalactites.Add(spawnedStalactite);

                // Assuming the stalactite needs to fall to the shadow's position
                StartCoroutine(MoveStalactiteToShadow(spawnedStalactite, spawnedShadow.transform.position));

                // Destroy the shadow after the duration
                Destroy(spawnedShadow, stalactiteDuration);
                Destroy(spawnedStalactite, stalactiteDuration);
            }
            yield return new WaitForSeconds(stalactiteSpawnGap);
        }

        // Optionally, wait for all stalactites to finish falling (if needed)
        // yield return new WaitUntil(() => stalactites.All(st => !st.activeSelf));

        crystalAnimationState.PlayEndAnimation();
        cgController.canMove = true;
        ResetAbilityStates();
    }

    // Coroutine to move stalactite to the shadow's position
    private IEnumerator MoveStalactiteToShadow(GameObject stalactite, Vector3 targetPosition)
    {
        float elapsedTime = 0f;

        Vector3 startPosition = stalactite.transform.position;

        while (elapsedTime < fallSpeed)
        {
            float t = elapsedTime / fallSpeed;
            stalactite.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the stalactite ends up exactly at the target position
        stalactite.transform.position = targetPosition;
    }

    private void EnableLaser()
    {
        crystal = GetClosestCrystal();
        crystalAnimationState.SetEnemyDirection();

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
        if (crystal == null || firePointLaser == null) return;

        // Update the positions of the line renderer
        tempLine.SetPosition(0, firePointLaser.position);
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
        tempStartVFX = Instantiate(startVFX, firePointLaser.position, Quaternion.identity);
        tempEndVFX = Instantiate(endVFX, crystal.position, Quaternion.identity);

        // Update the direction and check for hits
        Vector2 direction = (Vector2)crystal.position - (Vector2)firePointLaser.position;
        float distance = direction.magnitude;
        direction.Normalize();

        int laserMask = LayerMask.GetMask("TilemapColliders", "IgnoreCrystal");
        RaycastHit2D hit = Physics2D.Raycast(firePointLaser.position, direction, distance, laserMask);

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
