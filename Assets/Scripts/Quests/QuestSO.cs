
using UnityEngine;
[CreateAssetMenu(fileName = "Quest", menuName = "Quests/Quest")]
public class QuestSO : ScriptableObject
{
    [HideInInspector] public int questID;
    public string questName;
    [TextArea(3, 10)]
    public string questDescription;
    public enum QuestType { PRIMARY, SECONDARY }
    public QuestStepSO[] questSteps;
    public bool allStepsCompleted;
    public QuestCategorySO questCategory;

    public Item[] itemReward;
    // public int expReward;
    // public int goldReward;

}
