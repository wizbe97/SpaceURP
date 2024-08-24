using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private int waveValue;  // Shows the total budget for the current wave
    [SerializeField] private int remainingBudget;  // Shows the remaining budget during enemy generation

    public List<Enemy> enemies = new List<Enemy>();
    public int currWave;
    public List<GameObject> enemiesToSpawn = new List<GameObject>();

    public Transform[] spawnLocation;
    private int spawnIndex;
    public int difficulty;
    public float spawnDelay = 1.0f;

    private float waveDuration;
    private float waveTimer;
    private float spawnInterval;
    public bool startNextWave;

    public List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        GenerateWave();
        startNextWave = false;
    }

    void Update()
    {
        if (spawnedEnemies.Count <= 0 && waveTimer <= 0)
        {
            currWave++;
            GenerateWave();
        }
    }

    public void GenerateWave()
    {
        waveValue = currWave * difficulty;
        remainingBudget = waveValue;  // Set remaining budget to the initial wave value
        GenerateEnemies();

        spawnInterval = waveDuration / enemiesToSpawn.Count;
        waveTimer = waveDuration;

        StartCoroutine(SpawnEnemiesWithDelay());
    }

    public void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();
        while (remainingBudget > 0 || generatedEnemies.Count < 50)
        {
            int randEnemyId = Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randEnemyId].cost;

            if (remainingBudget - randEnemyCost >= 0)
            {
                generatedEnemies.Add(enemies[randEnemyId].enemyPrefab);
                remainingBudget -= randEnemyCost;  // Update remaining budget after each enemy is added
            }
            else if (remainingBudget <= 0)
            {
                break;
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }

    IEnumerator SpawnEnemiesWithDelay()
    {
        foreach (GameObject enemyPrefab in enemiesToSpawn)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnLocation[spawnIndex].position, Quaternion.identity);
            spawnedEnemies.Add(enemy);

            spawnIndex = (spawnIndex + 1) % spawnLocation.Length;

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}

[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    public int cost;
}
