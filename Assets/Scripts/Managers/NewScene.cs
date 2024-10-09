using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class NewScene : MonoBehaviour
{
    public string sceneName;
    public Animator transition;

    [SerializeField] private int cameraConfinerIndex;
    public static int cameraConfinerIndexToPass; // Static variable to persist across scenes
    public float transitionTime = 1f;
    public Transform spawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCollisions"))
        {
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        LoadNextLevel(sceneName, spawnPoint);
    }

    public void LoadNextLevel(string newSceneName, Transform spawnPoint)
    {
        GameManager.Instance.scenePlayerSpawnPosition = spawnPoint.position;
        GameManager.Instance.SaveAllData(isLocal: true);

        // Store the index to pass to the new scene
        cameraConfinerIndexToPass = cameraConfinerIndex;

        FindAnyObjectByType<PlayerController>().enabled = false;
        FindAnyObjectByType<UpdateAnimationState>().stateLock = true;
        GameManager.Instance.inventoryManager.gameObject.SetActive(false);
        GameManager.Instance.healthBarManager.gameObject.SetActive(false);
        
        // Start loading the new scene
        StartCoroutine(LoadLevel(newSceneName));
    }

    IEnumerator LoadLevel(string sceneToLoad)
    {
        transition.SetTrigger("Start");

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load the new scene
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
        transition.SetTrigger("Start");

        FindAnyObjectByType<PlayerController>().enabled = true;
        FindAnyObjectByType<UpdateAnimationState>().stateLock = false;
        GameManager.Instance.inventoryManager.gameObject.SetActive(true);
        GameManager.Instance.healthBarManager.gameObject.SetActive(true);
    }
}
