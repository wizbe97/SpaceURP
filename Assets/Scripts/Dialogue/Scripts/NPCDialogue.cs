using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    private SpriteRenderer speechBubbleRenderer;
    public DialogueSO[] conversation;
    private DialogueManager dialogueManager;
    private bool dialogueInitiated;
    void Start()
    {
        speechBubbleRenderer = GetComponent<SpriteRenderer>();
        speechBubbleRenderer.enabled = false;
        dialogueManager = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !dialogueInitiated)
        {
            speechBubbleRenderer.enabled = true;
            dialogueManager.InitiateDialogue(this);
            dialogueInitiated = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            speechBubbleRenderer.enabled = false;
            dialogueManager.TurnOffDialogue();
            dialogueInitiated = false;
        }
    }
}
