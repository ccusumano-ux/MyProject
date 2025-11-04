using UnityEngine;
using UnityEngine.Events;

public class PlayerLife : MonoBehaviour
{
    [Header("Life Settings")]
    public int maxLives = 3;
    public int currentLives;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1f;
    private bool isInvincible = false;

    [Header("Events")]
    public UnityEvent onTakeDamage;
    public UnityEvent onDeath;

    [HideInInspector] public bool debugDeathTriggered = false;

    private void Awake()
    {
        currentLives = maxLives;
    }

    private void Update()
    {
        // Debug: Press 'O' to instantly die and test leaderboard submission
        if (Input.GetKeyDown(KeyCode.O))
        {
            debugDeathTriggered = true;
            ForceDeathForDebug();
        }
    }

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
    }

    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    public void Heal(int amount = 1)
    {
        currentLives += amount;
        currentLives = Mathf.Min(currentLives, maxLives);
    }

    // Debug helper: kills player immediately
    private void ForceDeathForDebug()
    {
        currentLives = 0;
        Die();
    }

}
//Version 0001