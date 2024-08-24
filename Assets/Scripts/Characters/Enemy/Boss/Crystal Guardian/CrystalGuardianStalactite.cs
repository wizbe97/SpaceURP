using UnityEngine;
using System.Collections;

public class CrystalGuardianStalactite : MonoBehaviour
{
    [HideInInspector] public CircleCollider2D circleCollider2D;
    [SerializeField] private int stalactiteDamage = 10;
    [SerializeField] private float damageCooldown = 2f; // Time before the player can be damaged again
    private bool canDamage = true;

    private void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        StartCoroutine(ColliderEnable());
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (canDamage && other.collider.CompareTag("PlayerCollisions"))
        {
            // Access the GameObject from the collision and then get the Player component
            Player player = other.gameObject.GetComponent<Player>();
            if (player != null)
            {
                StartCoroutine(player.DamageCharacter(stalactiteDamage, 0)); // Set interval to 0 to ensure it's only called once
                StartCoroutine(ResetDamageCooldown());
            }
        }
    }

    private IEnumerator ColliderEnable()
    {
        yield return new WaitForSeconds(0.25f);
        EnableCollider();
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
