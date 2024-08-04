using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalGuardian : Boss
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Setup abilities specific to CrystalGuardian
    protected override void SetupAbilities()
    {
        AddAbility(SpecialAbility1);
        AddAbility(SpecialAbility2);
        AddAbility(SpecialAbility3);
    }

    // Example ability 1
    private void SpecialAbility1()
    {
        Debug.Log("CrystalGuardian uses Special Ability 1!");
        // Ability logic here
    }

    // Example ability 2
    private void SpecialAbility2()
    {
        Debug.Log("CrystalGuardian uses Special Ability 2!");
        // Ability logic here
    }

    private void SpecialAbility3()
    // Example ability 3
    {
        Debug.Log("CrystalGuardian uses Special Ability 3!");
        // Ability logic here
    }
}
