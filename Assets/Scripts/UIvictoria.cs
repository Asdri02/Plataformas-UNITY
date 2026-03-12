using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryUI : MonoBehaviour
{
    public void RestartLevel()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}