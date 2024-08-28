using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
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

    private void Start()
    {
        // Find Buttons
        optionsPanel = GameObject.Find("OptionsPanel");
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

    private void Update()
    {
        // Only allow "F" key input if all option buttons are deactivated
        if (dialogueActivated && Input.GetKeyDown(KeyCode.F) && AllOptionButtonsDeactivated())
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

        if (stepNum < currentConversation.dialogue.Length)
        {
            dialogueText.text = currentConversation.dialogue[stepNum];
        }
        else
        {
            optionsPanel.SetActive(true);
        }

        dialogueCanvas.SetActive(true);
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
        Debug.Log("Current character = " + currentSpeaker);
        Debug.Log("Current portrait = " + currentPortrait);
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
        foreach (GameObject button in optionButton)
        {
            button.SetActive(false);
        }

        if (optionNum == 0)
        {
            currentConversation = currentConversation.option0;
        }
        if (optionNum == 1)
        {
            currentConversation = currentConversation.option1;
        }
        if (optionNum == 2)
        {
            currentConversation = currentConversation.option2;
        }
        if (optionNum == 3)
        {
            currentConversation = currentConversation.option3;
        }
        stepNum = 0;
        PlayDialogue();
    }
    


    public void InitiateDialogue(NPCDialogue nPCDialogue)
    {
        // read the array we are currently stepping through
        currentConversation = nPCDialogue.conversation[0];
        Debug.Log("Started conversation");
        dialogueActivated = true;

    }

    public void TurnOffDialogue()
    {
        // reset the current conversation
        stepNum = 0;
        Debug.Log("Ended conversation");
        dialogueActivated = false;
        optionsPanel.SetActive(false);
        dialogueCanvas.SetActive(false);
    }
}

public enum DialogueActors
{
    Wizbe,
    Scientist,
    Random,
    Branch
};