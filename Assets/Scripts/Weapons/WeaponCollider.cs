using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    private Coroutine damageCoroutine;
    private WeaponInfo weaponInfo;

    private void Start() {
        weaponInfo = GetComponentInParent<WeaponInfo>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyCharacter enemy = other.gameObject.GetComponent<EnemyCharacter>();
            Rigidbody2D enemyRb = other.gameObject.GetComponent<Rigidbody2D>();

            // Apply knockback
            Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
            enemyRb.AddForce(knockbackDirection * weaponInfo.knockbackForce, ForceMode2D.Impulse);

            // Only call DamageCharacter on the Enemy if we don't currently have a DamageCharacter() Coroutine running.
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(enemy.DamageCharacter(weaponInfo.damage, 0f));
            }
        }
    }
}
