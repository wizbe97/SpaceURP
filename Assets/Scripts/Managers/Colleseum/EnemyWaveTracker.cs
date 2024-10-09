using UnityEngine;

public class EnemyWaveTracker : MonoBehaviour
{
    void OnDestroy()
    {
        GameObject waveSpawner = GameObject.FindGameObjectWithTag("WaveSpawner");
        if (waveSpawner != null)
        {
            WaveSpawner waveSpawnerScript = waveSpawner.GetComponent<WaveSpawner>();
            // Remove the reference to this enemy from the list before it's destroyed
            if (waveSpawnerScript.spawnedEnemies.Contains(gameObject))
            {
                waveSpawnerScript.spawnedEnemies.Remove(gameObject);
            }
        }
    }
}
