using TMPro;
using UnityEngine;

public class UIUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI livesText;

    private void Start()
    {
        GameManager.Instance.OnCoinsChanged += UpdateCoins;
        GameManager.Instance.OnLivesChanged += UpdateLives;

        UpdateCoins(GameManager.Instance.GetCoins());
        UpdateLives(GameManager.Instance.GetLives());
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCoinsChanged -= UpdateCoins;
            GameManager.Instance.OnLivesChanged -= UpdateLives;
        }
    }

    private void UpdateCoins(int coins)
    {
        if (coinsText != null)
            coinsText.text = "Monedas: " + coins;
    }

    private void UpdateLives(int lives)
    {
        if (livesText != null)
            livesText.text = "Vidas: " + lives;
    }
}