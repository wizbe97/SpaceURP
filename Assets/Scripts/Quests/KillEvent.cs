using UnityEngine;

public class KillEvent : MonoBehaviour
{
    [SerializeField] QuestStepEventSO killEvent;
    private EnemyCharacter enemyCharacter;
    private bool eventRaised = false;

    public void Awake()
    {
        enemyCharacter = GetComponentInParent<EnemyCharacter>();
    }

    private void Update()
    {
        if (eventRaised == false && enemyCharacter.hitPoints <= 0)
        {
            killEvent.Raise();
            Debug.Log("Kill Event Raised");
            eventRaised = true;
        }
    }
}
