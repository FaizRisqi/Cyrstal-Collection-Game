using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public Text scoreText;
    private int finalScore;

    void Start()
    {
        // Ambil score dari PlayerPrefs (disimpan dari GameManager)
        finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        
        if (scoreText != null)
        {
            scoreText.text = "Final Score: " + finalScore;
        }
    }

    public void RetryLevel()
    {
        Debug.Log("Retrying current level");
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        SceneManager.LoadScene("Level" + currentLevel);
    }

    public void BackToMainMenu()
    {
        Debug.Log("Back to Main Menu");
        SceneManager.LoadScene("MainMenu");
    }
}