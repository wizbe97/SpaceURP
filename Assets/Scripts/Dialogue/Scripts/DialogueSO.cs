using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue SO")]
public class DialogueSO : ScriptableObject
{
    public DialogueActors[] actors;
    [Tooltip("Only needed if Random is selected as an Actor")]
    [Header("Random Actor Info")]
    public string randomActorName;
    public Sprite randomActorPortrait;

    [Header("Dialogue")]
    [TextArea]
    public string[] dialogue;

    [Tooltip("The words that will appear on the options buttons")]
    [Header("Options")]
    public string[] optionText;
    public DialogueSO option0;
    public DialogueSO option1;
    public DialogueSO option2;
    public DialogueSO option3;

    public QuestSO quest;

    [Header("Option Events")]
    public QuestAction option0Action;
    public QuestAction option1Action;
    public QuestAction option2Action;
    public QuestAction option3Action;

    public enum QuestAction
    {
        None,
        StartQuest,
        CompleteQuest,
        UpdateQuestProgress
    }
}
