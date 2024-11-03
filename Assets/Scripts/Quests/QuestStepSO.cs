using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "QuestStep", menuName = "QuestSystem/Quest/QuestStep")]
public class QuestStepSO : ScriptableObject
{
    //To store what event we are listening to
    [SerializeField] QuestStepEventSO questEvent;
    //This will be the response executed when the event is raised
    UnityEvent response;
    //To keep track if step is completed
    public bool isStepCompleted = false;

    private void OnEnable()
    {
        //Registers the listerner
        questEvent.RegisterListeners(this);
        //Sets what we want to do in response to the event happening
        response?.AddListener(CompleteStep);
    }

    private void OnDisable()
    {
        //Unregisters the listener
        questEvent.UnregisterListener(this);
    }

    //We can right click in the Inspector, on this SO and then press Reset
    //to reset the variable to false
    private void Reset()
    {
        isStepCompleted = false;
    }

    public void OnEventRaised()
    {
        //The ? checks if response is not equal to null. If it is not, Invoke() the response.
        response?.Invoke();
    }

    public void CompleteStep()
    {
        Debug.Log("Quest Step Completed");
        isStepCompleted = true;
    }
}