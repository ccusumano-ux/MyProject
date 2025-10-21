using UnityEngine;
using UnityEngine.Events;

public class PlayerLife : MonoBehaviour
{
    [Header("Life Settings")]
    public int maxLives = 3;
    public int currentLives;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1f; // Time after hit player can't take damage
    private bool isInvincible = false;

    [Header("Events")]
    public UnityEvent onTakeDamage;  // Optional event for UI, effects
    public UnityEvent onDeath;

    private void Awake()
    {
        currentLives = maxLives;
    }

    /// <summary>
    /// Call this method to deal damage to the player.
    /// </summary>
    /// <param name="amount">Amount of life to remove</param>
    public void TakeDamage(int amount = 1)
    {
        if (isInvincible) return;

        currentLives -= amount;
        currentLives = Mathf.Max(currentLives, 0);

        onTakeDamage?.Invoke();

        if (currentLives <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    private void Die()
    {
        Debug.Log("💀 Player Died!");
        onDeath?.Invoke();

        // Optional: disable player controls
        // GetComponent<PlayerController>().enabled = false;
    }

    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    /// <summary>
    /// Heal the player
    /// </summary>
    public void Heal(int amount = 1)
    {
        currentLives += amount;
        currentLives = Mathf.Min(currentLives, maxLives);
    }
}
