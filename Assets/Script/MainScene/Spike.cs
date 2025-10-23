using UnityEngine;

public class Spike : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 1;                 // How much life to remove
    public bool oneTimeDamage = true;      // Only damage once per spike per player

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerLife playerLife = collision.GetComponent<PlayerLife>();
            if (playerLife != null)
            {
                playerLife.TakeDamage(damage);

                if (oneTimeDamage)
                {
                    // Optional: disable collider to prevent repeated damage
                    Collider2D col = GetComponent<Collider2D>();
                    if (col != null) col.enabled = false;
                }
            }
        }
    }
}
