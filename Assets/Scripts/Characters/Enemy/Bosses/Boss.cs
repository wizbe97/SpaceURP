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

    // Execute a random ability
    private void ExecuteRandomAbility()
    {
        if (abilities.Count > 0)
        {
            int randomIndex = Random.Range(0, abilities.Count);
            abilities[randomIndex]?.Invoke();
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
