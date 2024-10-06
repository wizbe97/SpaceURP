using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NewScene : MonoBehaviour
{
    public string sceneName;
    public Animator transition;

    private ConfinerManager confinerManager;
    [SerializeField] private int cameraConfinerIndex;
    public static int cameraConfinerIndexToPass; // Static variable to persist across scenes
    public float transitionTime = 1f;
    public Transform spawnPoint;

    private void Awake()
    {
        confinerManager = FindObjectOfType<ConfinerManager>();
    }

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

        // Store the index to pass to the new scene
        cameraConfinerIndexToPass = cameraConfinerIndex;

        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        transition.SetTrigger("Start");
        FindAnyObjectByType<PlayerController>().canMove = false;
        FindAnyObjectByType<UpdateAnimationState>().stateLock = true;

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load scene
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        FindAnyObjectByType<PlayerController>().canMove = true;
        FindAnyObjectByType<UpdateAnimationState>().stateLock = false;
    }
}
