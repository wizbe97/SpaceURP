using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalGuardianMeleeWeapon : MonoBehaviour
{
    private CrystalGuardianAttack crystalGuardianAttack;

    private void Start()
    {
        crystalGuardianAttack = GetComponentInParent<CrystalGuardianAttack>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Get the Player component and apply damage
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            Debug.Log("Damgaged Player");
            StartCoroutine(player.DamageCharacter(crystalGuardianAttack.attackDamage, 0)); // Set interval to 0 to ensure it's only called once
        }

    }
}
