using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NewScene : MonoBehaviour
{
    public string sceneName;
    public Animator transition;
    public float transitionTime = 1f;
    public Transform spawnPoint; // Reference to the spawn point Transform

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCollisions"))
        {
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        GameManager.Instance.scenePlayerSpawnPosition = spawnPoint.position;
        GameManager.Instance.SaveAllData(isLocal: true);
        // // Send a message to PlayerManager with spawn point information
        // PlayerManager.Instance.SendMessage("SetSpawnPoint", spawnPoint.position);

        // Load the new scene
        StartCoroutine(LoadLevel());

    }

    IEnumerator LoadLevel()
    {
        transition.SetTrigger("Start");
        FindAnyObjectByType<PlayerController>().canMove = false;

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load scene
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        FindAnyObjectByType<PlayerController>().canMove = true;

    }
}
