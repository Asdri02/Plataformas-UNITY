using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private int initialLives = 3;

    private int coins = 0;
    private int lives;

    public Action<int> OnCoinsChanged;
    public Action<int> OnLivesChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        lives = initialLives;
    }

    public void AddCoin()
    {
        coins++;
        OnCoinsChanged?.Invoke(coins);
    }

    public void LoseLife()
    {
        lives--;
        OnLivesChanged?.Invoke(lives);

        if (lives <= 0)
        {
            RestartLevel();
        }
    }

    public int GetCoins()
    {
        return coins;
    }

    public int GetLives()
    {
        return lives;
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}