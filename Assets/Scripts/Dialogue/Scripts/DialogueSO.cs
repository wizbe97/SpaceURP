using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
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


    [Header("Option Events")]
    public OptionAction option0Action;
    public OptionAction option1Action;
    public OptionAction option2Action;
    public OptionAction option3Action;

    public enum OptionAction
    {
        None,
        GiveQuest,
        CompleteQuest,
        UpdateQuest,
        EndDialogue,
        OpenGate
    }
}