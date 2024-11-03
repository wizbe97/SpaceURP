using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestManager", menuName = "Managers/QuestManager")]
public class QuestManagerSO : ScriptableObject
{
    public QuestSO primaryQuest;
    public List<QuestSO> activeQuests = new();
    public List<QuestSO> completedQuests = new();

    public void AddQuest(QuestSO quest) {
        activeQuests.Add(quest);
    }

    public void CompleteQuest(QuestSO quest) {
        activeQuests.Remove(quest);
        completedQuests.Add(quest);
    }
}
