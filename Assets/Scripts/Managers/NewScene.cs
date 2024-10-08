using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
        LoadNextLevel(sceneName, spawnPoint); // Call overloaded method with the predefined scene name
    }

    public void LoadNextLevel(string newSceneName, Transform spawnPoint)
    {
        GameManager.Instance.scenePlayerSpawnPosition = spawnPoint.position;
        GameManager.Instance.SaveAllData(isLocal: true);

        // Store the index to pass to the new scene
        cameraConfinerIndexToPass = cameraConfinerIndex;

        // Start loading the new scene
        StartCoroutine(LoadLevel(newSceneName));
    }

    IEnumerator LoadLevel(string sceneToLoad)
    {
        transition.SetTrigger("Start");
        FindAnyObjectByType<PlayerController>().canMove = false;
        FindAnyObjectByType<UpdateAnimationState>().stateLock = true;

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load the new scene
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);

        FindAnyObjectByType<PlayerController>().canMove = true;
        FindAnyObjectByType<UpdateAnimationState>().stateLock = false;
    }
}
