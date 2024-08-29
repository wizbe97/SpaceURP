using System.Collections.Generic;
using UnityEngine;

public class SpawnNPCs : MonoBehaviour
{
    // List of NPC prefabs to instantiate
    public List<GameObject> npcPrefabs;

    // List of transform points where NPCs can be spawned
    public List<Transform> spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        // Make a copy of the spawnPoints list to keep track of available spawn points
        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);

        // Loop through each NPC prefab in the list
        foreach (GameObject npc in npcPrefabs)
        {
            if (availableSpawnPoints.Count == 0)
            {
                Debug.LogWarning("Not enough spawn points for all NPCs.");
                break;
            }

            // Pick a random spawn point from the available points
            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            Transform spawnPoint = availableSpawnPoints[randomIndex];

            // Instantiate the NPC at the selected spawn point
            Instantiate(npc, spawnPoint.position, spawnPoint.rotation);

            // Remove the selected spawn point from the available list
            availableSpawnPoints.RemoveAt(randomIndex);
        }
    }
}
