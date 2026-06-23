using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public void LoadLevel1()
    {
        Debug.Log("Loading Level 1");
        SceneManager.LoadScene("Level1");
    }

    public void LoadLevel2()
    {
        Debug.Log("Loading Level 2");
        SceneManager.LoadScene("Level2");
    }

    public void BackToMainMenu()
    {
        Debug.Log("Back to Main Menu");
        SceneManager.LoadScene("MainMenu");
    }
}