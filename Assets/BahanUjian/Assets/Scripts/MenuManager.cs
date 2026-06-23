using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("Play clicked - Loading LevelSelect");
        SceneManager.LoadScene("LevelSelect");
    }

    public void SelectLevel()
    {
        Debug.Log("Select Level clicked - Loading LevelSelect");
        SceneManager.LoadScene("LevelSelect");
    }

    public void QuitGame()
    {
        Debug.Log("Quit game");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}