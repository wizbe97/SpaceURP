using System.Collections;
using UnityEngine;

public class OpenGate : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D gateCollider;
    [SerializeField] private Rigidbody2D gateRigidbody;
    [SerializeField] private string sceneToLoad;
    [SerializeField] private Transform spawnPoint;

    private void Awake()
    {
        gateRigidbody.bodyType = RigidbodyType2D.Kinematic;
    }

    public void OpenTheGate()
    {
        StartCoroutine(OpenGateAfterDelay());
    }

    private IEnumerator CloseGateAfterDelay()
    {
        // Wait for 10 seconds
        yield return new WaitForSeconds(10f);

        // Close the gate
        animator.SetBool("isOpen", false);
        gateCollider.enabled = true;
    }

    private IEnumerator OpenGateAfterDelay()
    {
        // Wait for 1 seconds
        yield return new WaitForSeconds(1f);

        // Open the gate
        animator.SetBool("isOpen", true);
        gateCollider.enabled = false;
        StartCoroutine(CloseGateAfterDelay());

        yield return new WaitForSeconds(1f);
        NewScene newScene = FindObjectOfType<NewScene>();
        newScene.LoadNextLevel(sceneToLoad, spawnPoint);
    }
}
