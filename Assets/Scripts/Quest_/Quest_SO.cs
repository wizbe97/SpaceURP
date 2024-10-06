using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Quest_SO : ScriptableObject
{
    [System.Serializable]
    public struct Info
    {
        public int ID;
        public string name;
        public string description;
    }

    [Header("Quest Info")] public Info information;
    public struct Stat
    {
        public int currency;
        public int xp;
    }
    [Header("Rewards")] public Stat rewards = new Stat { currency = 10, xp = 10 };

    public bool completed { get; protected set; }
    public QuestCompletedEvent questCompleted;

    public abstract class QuestGoal : ScriptableObject
    {
        protected string description;
        public int currentAmount { get; protected set; }
        public int requiredAmount = 1;

        public bool completed { get; protected set; }
        [HideInInspector] public UnityEvent goalCompleted;

        public virtual string GetDescription()
        {
            return description;
        }

        public virtual void Initialise()
        {
            completed = false;
            goalCompleted = new UnityEvent();
        }

        protected void Evaluate()
        {
            if (currentAmount >= requiredAmount)
            {
                Complete();
            }
        }

        private void Complete()
        {
            completed = true;
            goalCompleted.Invoke();
            goalCompleted.RemoveAllListeners();
        }

        public void Skip()
        {
            // charge the player some currency
            Complete();
        }
    }

    public List<QuestGoal> goals;

    public void Initialise()
    {
        completed = false;
        questCompleted = new QuestCompletedEvent();

        foreach (QuestGoal goal in goals)
        {
            goal.Initialise();
            goal.goalCompleted.AddListener(delegate { CheckGoals(); });
        }
    }

    private void CheckGoals() {
        completed = goals.All(goal => goal.completed);
        if (completed)
        {
            // give reward
            questCompleted.Invoke(this);
            questCompleted.RemoveAllListeners();
        }
    }
}

public class QuestCompletedEvent : UnityEvent<Quest_SO> { }