using UnityEngine;
using System.Collections.Generic;


public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public Dictionary<string, QuestSO> activeQuests = new Dictionary<string, QuestSO>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void StartQuest(QuestSO quest)
    {
        if (!activeQuests.ContainsKey(quest.questName))
        {
            // Add the quest to the active quests dictionary
            activeQuests.Add(quest.questName, quest);
            Debug.Log(quest.questName + " Quest started!");
        }
        else
        {
            Debug.Log("Quest " + quest.questName + " is already active!");
        }

    }

    public void CompleteQuest(QuestSO quest)
    {
        Debug.Log("Quest completed!");
        activeQuests.Remove(quest.questName);
    }

    public void UpdateQuestProgress()
    {
        Debug.Log("Quest progress updated!");
    }
}
