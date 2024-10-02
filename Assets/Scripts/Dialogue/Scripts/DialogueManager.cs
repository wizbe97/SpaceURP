using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    private PlayerController playerController;
    private UpdateAnimationState updateAnimationState;

    private NPCDialogue npcDialogue;
    // The NPC Dialogue we are currently stepping through
    private DialogueSO currentConversation;
    private int stepNum;
    private bool dialogueActivated;

    // UI References
    private GameObject dialogueCanvas;
    private TMP_Text actor;
    private Image portrait;
    private TMP_Text dialogueText;

    private string currentSpeaker;
    private Sprite currentPortrait;
    public ActorSO[] actorSO;

    // Button References
    [SerializeField] private GameObject[] optionButton;
    private TMP_Text[] optionButtonText;
    private GameObject optionsPanel;

    // Typewriter Effect
    [SerializeField] private float typingSpeed = 0.02f;
    private Coroutine typeWriterRoutine;
    private bool canContinueText = true;

    private void Start()
    {
        npcDialogue = FindObjectOfType<NPCDialogue>();
        playerController = FindObjectOfType<PlayerController>();
        updateAnimationState = FindObjectOfType<UpdateAnimationState>();
        // Find Buttons
        optionsPanel = GameObject.Find("OptionsPanel");
        optionButton[0] = GameObject.Find("Option 0");
        optionButton[1] = GameObject.Find("Option 1");
        optionButton[2] = GameObject.Find("Option 2");
        optionButton[3] = GameObject.Find("Option 3");
        optionsPanel.SetActive(false);

        // // Find the TMP Text on the Buttons
        optionButtonText = new TMP_Text[optionButton.Length];
        for (int i = 0; i < optionButton.Length; i++)
        {
            optionButtonText[i] = optionButton[i].GetComponentInChildren<TMP_Text>();
        }

        // Turn off the buttons at the start of the game
        for (int i = 0; i < optionButton.Length; i++)
        {
            optionButton[i].SetActive(false);
        }

        dialogueCanvas = GameObject.Find("DialogueCanvas");
        actor = GameObject.Find("ActorText").GetComponent<TMP_Text>();
        portrait = GameObject.Find("Portrait").GetComponent<Image>();
        dialogueText = GameObject.Find("DialogueText").GetComponent<TMP_Text>();

        dialogueCanvas.SetActive(false);
    }

    public void StartDialogue()
    {
        if (dialogueActivated && Input.GetKeyDown(KeyCode.F) && canContinueText && AllOptionButtonsDeactivated())
        {
            if (stepNum >= currentConversation.actors.Length)
            {
                TurnOffDialogue();
            }
            else
            {
                PlayDialogue();
            }
        }
    }

    private void PlayDialogue()
    {
        // If it's a random character
        if (currentConversation.actors[stepNum] == DialogueActors.Random)
        {
            SetActorInfo(false);
        }
        // If it's a recurring character
        else
        {
            SetActorInfo(true);
        }

        // Display Dialogue
        actor.text = currentSpeaker;
        portrait.sprite = currentPortrait;

        // If there is an options branch..
        if (currentConversation.actors[stepNum] == DialogueActors.Branch)
        {
            optionsPanel.SetActive(true);
            for (int i = 0; i < currentConversation.optionText.Length; i++)
            {
                if (currentConversation.optionText[i] == null)
                {
                    optionButton[i].SetActive(false);
                }
                else
                {
                    optionButtonText[i].text = currentConversation.optionText[i];
                    optionButton[i].SetActive(true);
                }
            }

            // Stop the dialogue from progressing automatically
            return;
        }

        // Keep the routine from running multiple times at the same time
        if (typeWriterRoutine != null)
        {
            StopCoroutine(typeWriterRoutine);
        }

        if (stepNum < currentConversation.dialogue.Length)
        {
            typeWriterRoutine = StartCoroutine(TypeWriterEffect(dialogueText.text = currentConversation.dialogue[stepNum]));
        }
        else
        {
            optionsPanel.SetActive(true);
        }

        dialogueCanvas.SetActive(true);

        GameManager.Instance.inventoryManager.gameObject.SetActive(false);
        GameManager.Instance.healthBarManager.gameObject.SetActive(false);

        playerController.canMove = false;
        updateAnimationState.stateLock = true;
        

        stepNum += 1;
    }

    // Helper method to check if all option buttons are deactivated
    private bool AllOptionButtonsDeactivated()
    {
        foreach (GameObject button in optionButton)
        {
            if (button.activeSelf)
            {
                return false; // If any button is active, return false
            }
        }
        return true; // All buttons are inactive, return true
    }

    private void SetActorInfo(bool recurringCharacter)
    {
        if (recurringCharacter)
        {
            for (int i = 0; i < actorSO.Length; i++)
            {
                if (actorSO[i].name == currentConversation.actors[stepNum].ToString())
                {
                    currentSpeaker = actorSO[i].actorName;
                    currentPortrait = actorSO[i].actorPortrait;
                }
            }
        }
        else
        {
            currentSpeaker = currentConversation.randomActorName;
            currentPortrait = currentConversation.randomActorPortrait;
        }

    }

    public void Option(int optionNum)
    {
        // Deactivate all option buttons
        foreach (GameObject button in optionButton)
        {
            button.SetActive(false);
        }

        // Trigger the action based on the option selected
        DialogueSO.QuestAction action = DialogueSO.QuestAction.None;

        switch (optionNum)
        {
            case 0:
                action = currentConversation.option0Action;
                currentConversation = currentConversation.option0;
                break;
            case 1:
                action = currentConversation.option1Action;
                currentConversation = currentConversation.option1;
                break;
            case 2:
                action = currentConversation.option2Action;
                currentConversation = currentConversation.option2;
                break;
            case 3:
                action = currentConversation.option3Action;
                currentConversation = currentConversation.option3;
                break;
        }

        // Execute the corresponding quest action
        ExecuteQuestAction(action);

        // Reset the step number and continue dialogue
        stepNum = 0;
        PlayDialogue();
    }

    private void ExecuteQuestAction(DialogueSO.QuestAction action)
    {
        if (QuestManager.Instance == null)
        {
            Debug.LogError("QuestManager instance is not found in the scene.");
            return;
        }

        switch (action)
        {
            case DialogueSO.QuestAction.StartQuest:
                QuestManager.Instance.StartQuest();
                break;
            case DialogueSO.QuestAction.CompleteQuest:
                QuestManager.Instance.CompleteQuest();
                break;
            case DialogueSO.QuestAction.UpdateQuestProgress:
                QuestManager.Instance.UpdateQuestProgress();
                break;
            default:
                break;
        }
    }

    private IEnumerator TypeWriterEffect(string line)
    {
        dialogueText.text = "";
        canContinueText = false;
        bool addingRichTextTag = false;

        yield return new WaitForSeconds(.5f);
        foreach (char letter in line.ToCharArray())
        {
            if (Input.GetKey(KeyCode.F))
            {
                dialogueText.text = line;
                break;
            }

            // Check to see if we are working with a rich text tag
            if (letter == '<' || addingRichTextTag)
            {
                addingRichTextTag = true;
                dialogueText.text += letter;
                if (letter == '>')
                {
                    addingRichTextTag = false;
                }
            }

            else
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

        }
        canContinueText = true;
    }

    public void InitiateDialogue(NPCDialogue nPCDialogue, int dialogueIndex)
    {
        // read the array we are currently stepping through
        currentConversation = nPCDialogue.conversation[dialogueIndex];
        dialogueActivated = true;

    }

    public void TurnOffDialogue()
    {
        // Reset the current conversation
        stepNum = 0;
        dialogueActivated = false;
        npcDialogue.dialogueInitiated = false;
        optionsPanel.SetActive(false);
        dialogueCanvas.SetActive(false);

        // Safely check if inventoryManager and healthBarManager are not null before accessing them
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.inventoryManager != null)
            {
                GameManager.Instance.inventoryManager.gameObject.SetActive(true);
            }

            if (GameManager.Instance.healthBarManager != null)
            {
                GameManager.Instance.healthBarManager.gameObject.SetActive(true);
            }
        }

        playerController.canMove = true;
        updateAnimationState.stateLock = false;
    }


}

public enum DialogueActors
{
    Wizbe,
    Scientist,
    Random,
    Branch
};