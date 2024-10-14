using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();
    public int currWave;
    private int waveValue;
    public List<GameObject> enemiesToSpawn = new List<GameObject>();

    public Transform[] spawnLocation;
    public int spawnIndex;

    public int waveDuration;
    private float waveTimer;
    private float spawnInterval;
    private float spawnTimer;

    public List<GameObject> spawnedEnemies = new List<GameObject>();

    public float waveCooldown = 5f;  // Time between waves
    private float waveCooldownTimer = 0f;

    void Start()
    {
        GenerateWave();
    }

    void FixedUpdate()
    {
        // If wave cooldown is active, decrement timer
        if (waveCooldownTimer > 0)
        {
            waveCooldownTimer -= Time.fixedDeltaTime;
            return; // Skip the rest of the logic until the cooldown is done
        }

        if (spawnTimer <= 0)
        {
            if (enemiesToSpawn.Count > 0)
            {
                GameObject enemy = Instantiate(enemiesToSpawn[0], spawnLocation[spawnIndex].position, Quaternion.identity);
                enemiesToSpawn.RemoveAt(0);
                spawnedEnemies.Add(enemy);
                spawnTimer = spawnInterval;

                if (spawnIndex + 1 <= spawnLocation.Length - 1)
                {
                    spawnIndex++;
                }
                else
                {
                    spawnIndex = 0;
                }
            }
            else
            {
                waveTimer = 0;
            }
        }
        else
        {
            spawnTimer -= Time.fixedDeltaTime;
            waveTimer -= Time.fixedDeltaTime;
        }
        CleanUpSpawnedEnemies();

        // Check if the wave is over and no enemies are left
        if (waveTimer <= 0 && spawnedEnemies.Count <= 0)
        {
            StartWaveCooldown();  // Start the cooldown before the next wave
        }
    }

    private void StartWaveCooldown()
    {
        waveCooldownTimer = waveCooldown;  // Set the cooldown timer
        currWave++;                        // Move to the next wave
        Invoke("GenerateWave", waveCooldown); // Call GenerateWave() after the cooldown
    }

    public void GenerateWave()
    {
        waveValue = currWave * 3 + currWave + 5;
        GenerateEnemies();

        spawnInterval = waveDuration / enemiesToSpawn.Count;
        waveTimer = waveDuration;
    }

    public void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();
        while (waveValue > 0 || generatedEnemies.Count < 50)
        {
            int randEnemyId = Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randEnemyId].cost;

            if (waveValue - randEnemyCost >= 0)
            {
                generatedEnemies.Add(enemies[randEnemyId].enemyPrefab);
                waveValue -= randEnemyCost;
            }
            else if (waveValue <= 0)
            {
                break;
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }

    // Method to clean up null references from the spawnedEnemies list
    public void CleanUpSpawnedEnemies()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);
    }
}

[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    public int cost;
}
