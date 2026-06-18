using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Bisa dikembalikan nanti untuk Level Select
    public void PlayGame()
    {
        Debug.Log("Play clicked - Loading Level1");
        SceneManager.LoadScene("Level1");
    }

    public void SelectLevel()
    {
        Debug.Log("Select Level clicked");
        // Nanti buat LevelSelectScene untuk ini
        // SceneManager.LoadScene("LevelSelect");
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