using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestEvent", menuName = "QuestSystem/Quest/QuestEvent")]
public class QuestStepEventSO : ScriptableObject
{
    private List<QuestStepSO> listeners = new List<QuestStepSO>();


    //The Raise() method will tell all the subscribers to Invoke() their response.
    //It is basically a way to say "Hello! If you are subscribed, it is time to execute the event you wanted to..."
    public void Raise()
    {
        //It goes through all the listeners, and notifies
        //them that the event has been raise
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }

    public void RegisterListeners(QuestStepSO listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(QuestStepSO listener)
    {
        listeners.Remove(listener);
    }
}
