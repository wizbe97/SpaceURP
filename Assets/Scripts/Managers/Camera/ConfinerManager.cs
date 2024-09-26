using Cinemachine;
using UnityEngine;
using System.Collections;


public class ConfinerManager : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D[] regions;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private CinemachineConfiner confiner;

    private void Start()
    {
        StartCoroutine(InitializeConfiner());
    }

    private IEnumerator InitializeConfiner()
    {
        yield return null;

        UpdateConfinerCollider(NewScene.cameraConfinerIndexToPass);
    }

    public void UpdateConfinerCollider(int index)
    {
        if (confiner != null && index >= 0 && index < regions.Length)
        {
            confiner.m_BoundingShape2D = regions[index];
            confiner.InvalidatePathCache();
        }
        else
        {
            Debug.LogWarning("Confiner is not assigned or index is out of range.");
        }
    }
}
