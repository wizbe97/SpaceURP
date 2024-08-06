using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : MonoBehaviour
{
    // Timer duration for choosing a random ability
    [SerializeField] private float abilityCooldown = 5.0f;

    // Timer variable
    private float abilityTimer;

    // List of all available abilities
    private List<System.Action> abilities;

    // Track the index of the last used ability
    private int lastUsedAbilityIndex = -1;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Initialize the ability timer
        abilityTimer = abilityCooldown;

        // Initialize the abilities list
        abilities = new List<System.Action>();
        SetupAbilities();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // Countdown the timer
        abilityTimer -= Time.deltaTime;

        // Check if the timer has reached zero
        if (abilityTimer <= 0)
        {
            // Choose a random ability and execute it
            ExecuteRandomAbility();

            // Reset the timer
            abilityTimer = abilityCooldown;
        }
    }

    // Setup abilities (to be implemented in derived classes)
    protected abstract void SetupAbilities();

    // Execute a random ability, ensuring it's not the same as the last used ability
    private void ExecuteRandomAbility()
    {
        if (abilities.Count > 1)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, abilities.Count);
            } while (randomIndex == lastUsedAbilityIndex);

            abilities[randomIndex]?.Invoke();
            lastUsedAbilityIndex = randomIndex;
        }
        else if (abilities.Count == 1)
        {
            // If there's only one ability, just execute it
            abilities[0]?.Invoke();
        }
    }

    // Add an ability to the abilities list
    protected void AddAbility(System.Action ability)
    {
        if (ability != null)
        {
            abilities.Add(ability);
        }
    }
}