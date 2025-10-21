using TMPro; // add this at the top
using UnityEngine;

public class LifeUIManager : MonoBehaviour
{
    [Header("Player Reference")]
    public PlayerLife playerLife;

    [Header("UI Element")]
    public TextMeshProUGUI livesText;  // <-- change type here

    private void Start()
    {
        UpdateLivesUI();
    }

    private void OnEnable()
    {
        if (playerLife != null)
        {
            playerLife.onTakeDamage.AddListener(UpdateLivesUI);
            playerLife.onDeath.AddListener(UpdateLivesUI);
        }
    }

    private void OnDisable()
    {
        if (playerLife != null)
        {
            playerLife.onTakeDamage.RemoveListener(UpdateLivesUI);
            playerLife.onDeath.RemoveListener(UpdateLivesUI);
        }
    }

    public void UpdateLivesUI()
    {
        if (playerLife != null && livesText != null)
        {
            livesText.text = $"Lives: {playerLife.currentLives}/{playerLife.maxLives}";
        }
    }
}
