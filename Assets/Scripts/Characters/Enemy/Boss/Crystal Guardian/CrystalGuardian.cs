using System.Collections;
using UnityEngine;

public class CrystalGuardian : Boss
{
    [SerializeField] private GameObject crystalSpikes; // Spike prefab
    [SerializeField] private float spawnGap = 2.0f; // Gap time between spawns
    [SerializeField] private int minNumberOfSpikes = 3; // Minimum number of spikes to spawn
    [SerializeField] private int maxNumberOfSpikes = 10; // Maximum number of spikes to spawn
    [SerializeField] private float crystalSpikeDuration = 3.0f; // Duration before crystal spikes are destroyed

    private Transform player; // Player's transform
    private bool isSpawningSpikes = false; // Flag to indicate if spikes are being spawned

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Find the player GameObject by tag
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
            // Ability logic here
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
            // Ability logic here
        }
        else
        {
            Debug.Log("Cannot use ability while spikes are spawning.");
        }
    }
}
