using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quest")]
public class QuestSO : ScriptableObject
{
    public string questName; // The name of the quest
    [TextArea] public string questDescription; // A brief description of the quest
    public int goldReward; // The reward for completing the quest
    public bool isCompleted; // Whether the entire quest is completed

    // Nested class to represent each quest step
    [System.Serializable]
    public class QuestStep
    {
        [TextArea] public string description; // Description of the quest step
        public bool requirePreviousStep; // Whether this step requires the previous step to be completed
        public bool isCompleted; // Whether this step is completed
    }

    // List of steps for this quest
    public List<QuestStep> questSteps;

    // Method to check if all steps are completed
    public bool AreAllStepsCompleted()
    {
        foreach (QuestStep step in questSteps)
        {
            if (!step.isCompleted)
            {
                return false;
            }
        }
        return true;
    }

    // Method to mark the next incomplete step as completed
public void CompleteNextStep()
{
    for (int i = 0; i < questSteps.Count; i++)
    {
        QuestStep step = questSteps[i];

        // Skip if the step is already completed
        if (step.isCompleted)
        {
            continue;
        }

        // Check if the current step requires the previous step to be completed
        if (step.requirePreviousStep && i > 0)
        {
            // If the previous step is not completed, exit the method
            if (!questSteps[i - 1].isCompleted)
            {
                Debug.Log("Cannot complete this step. Previous step is not completed.");
                return;
            }
        }

        // Mark the current step as completed
        step.isCompleted = true;
        Debug.Log("Step completed: " + step.description);

        // Check if this was the last step to mark the entire quest as completed
        isCompleted = AreAllStepsCompleted();

        return;
    }
}

}
