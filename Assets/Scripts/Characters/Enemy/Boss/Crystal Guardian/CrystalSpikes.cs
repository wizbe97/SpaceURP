using UnityEngine;
using System.Collections;

public class CrystalSpikes : MonoBehaviour
{
    private CircleCollider2D circleCollider2D;
    [SerializeField] private int spikeDamage = 10;
    [SerializeField] private float damageCooldown = 2f; // Time before the player can be damaged again
    private bool canDamage = true;

    private void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canDamage)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                StartCoroutine(player.DamageCharacter(spikeDamage, 0)); // Set interval to 0 to ensure it's only called once
                StartCoroutine(ResetDamageCooldown());
            }
        }
    }

    private IEnumerator ResetDamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }

    public void EnableCollider()
    {
        circleCollider2D.enabled = true;
    }

    public void DisableCollider()
    {
        circleCollider2D.enabled = false;
    }
}
