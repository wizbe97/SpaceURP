using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    private Coroutine damageCoroutine;
    private WeaponInfo weaponInfo;

    // Dictionary to store hit enemies and the last time they were hit
    private Dictionary<EnemyCharacter, float> hitEnemies = new Dictionary<EnemyCharacter, float>();

    private PlayerAttack playerAttack; // Reference to playerAttack for spearCooldown

    private void Start()
    {
        weaponInfo = GetComponentInParent<WeaponInfo>();
        playerAttack = GetComponentInParent<PlayerAttack>(); // Assuming PlayerAttack is on the same parent object
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyCharacter enemy = other.gameObject.GetComponent<EnemyCharacter>();
            Rigidbody2D enemyRb = other.gameObject.GetComponent<Rigidbody2D>();

            // Check if the enemy has been hit recently
            if (CanDamageEnemy(enemy))
            {
                // Apply knockback
                Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
                enemyRb.AddForce(knockbackDirection * weaponInfo.knockbackForce, ForceMode2D.Impulse);

                // Damage the enemy
                if (damageCoroutine == null)
                {
                    damageCoroutine = StartCoroutine(enemy.DamageCharacter(weaponInfo.damage, 0f));
                }

                // Record the time when this enemy was hit
                hitEnemies[enemy] = Time.time;
            }
        }
    }

    private bool CanDamageEnemy(EnemyCharacter enemy)
    {
        if (hitEnemies.ContainsKey(enemy))
        {
            // Check if enough time has passed since the last hit
            float lastHitTime = hitEnemies[enemy];
            if (Time.time - lastHitTime < playerAttack.spearCooldown)
            {
                return false; // Still within cooldown, cannot damage yet
            }
        }

        return true; // Either not hit before or cooldown has passed
    }
}
