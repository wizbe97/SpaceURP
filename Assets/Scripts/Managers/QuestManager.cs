using UnityEngine;

public class QuestManager : MonoBehaviour
{
    // Singleton instance
    public static QuestManager Instance;

    private void Awake()
    {
        // Implement the singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Optionally, make this object persist between scenes
        DontDestroyOnLoad(gameObject);
    }

    public void StartQuest()
    {
        Debug.Log("Quest started!");
        // Implement the logic to start a quest here
    }

    public void CompleteQuest()
    {
        Debug.Log("Quest completed!");
        // Implement the logic to complete the quest here
    }

    public void UpdateQuestProgress()
    {
        Debug.Log("Quest progress updated!");
        // Implement quest progress update logic here
    }
}
