using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float level1Time = 120f;
    [SerializeField] private float level2Time = 150f;
    [SerializeField] private int minimumScoreToWin = 100;

    [Header("UI References")]
    [SerializeField] private Text timerText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text levelText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text gameOverMessageText;
    [SerializeField] private GameObject successPanel;
    [SerializeField] private Text successMessageText;

    [Header("UI Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button quitButton;

    [Header("Level Scenes")]
    [SerializeField] private string level1SceneName = "Level1";
    [SerializeField] private string level2SceneName = "Level2";

    private int score = 0;
    private float timeRemaining;
    private bool gameIsActive = true;
    private int totalCollectibles = 999; // Set angka besar dulu agar tidak menang otomatis di detik ke-0
    private int collectedCount = 0;

    // SINGLETON - Prevent duplicate GameManagers
    private static GameManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        // FIXED: Auto-detect level from scene name
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Current Scene: " + currentSceneName);
        
        if (currentSceneName.ToLower().Contains("level2") || currentSceneName.ToLower().Contains("2"))
        {
            currentLevel = 2;
        }
        else
        {
            currentLevel = 1;
        }

        // Initialize timer
        timeRemaining = (currentLevel == 1) ? level1Time : level2Time;
        
        // IMPORTANT: Reset time scale (in case it was paused)
        Time.timeScale = 1f;
        
        UpdateUI();

        // BAGIAN INI DIHAPUS karena sering salah hitung di Start
        // totalCollectibles = FindObjectsOfType<Collectible>().Length; 

        // Hide panels
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (successPanel != null) successPanel.SetActive(false);

        // Setup button listeners
        SetupButtons();

        // Game is active
        gameIsActive = true;
        
        // Show cursor during panels only
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void SetupButtons()
    {
        // FIXED: Remove all old listeners first, then add new ones
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartLevel);
        }
        
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(LoadNextLevel);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    void Update()
    {
        // FIXED: Only update timer when game is active
        if (!gameIsActive) return;

        // Decrease timer
        timeRemaining -= Time.deltaTime;

        // Check time up
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            CheckGameOver();
            return; // Stop updating after game over
        }

        // Warning color
        if (timeRemaining <= 30f && timerText != null)
        {
            timerText.color = Color.red;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        if (levelText != null)
        {
            levelText.text = "Level " + currentLevel;
        }
    }

    public void AddScore(int points)
    {
        if (!gameIsActive) return;
        
        score += points;
        collectedCount++;

        Debug.Log("Score: " + score + " | Collected: " + collectedCount + "/" + totalCollectibles);

        // FIXED: Tambahkan UpdateUI disini agar skor langsung berubah di layar
        UpdateUI();

        // FIXED: Cek menang - Hanya jika total sudah benar dihitung
        if (collectedCount >= totalCollectibles && totalCollectibles > 0)
        {
            Debug.Log("All collectibles collected! Showing success.");
            ShowSuccess();
        }
    }

    public void AddTime(int seconds)
    {
        if (!gameIsActive) return;
        
        timeRemaining += seconds;
        Debug.Log("Time bonus: +" + seconds + "s");
    }

    void ShowSuccess()
    {
        // FIXED: Stop game immediately
        gameIsActive = false;
        Time.timeScale = 0f; // Pause game

        Debug.Log("ShowSuccess called - Level " + currentLevel);

        if (successPanel != null)
        {
            successPanel.SetActive(true);
            
            if (successMessageText != null)
            {
                successMessageText.text = "Level " + currentLevel + " Complete!\n" +
                                         "Score: " + score + "\n" +
                                         "Time: " + Mathf.FloorToInt(timeRemaining) + "s";
            }

            // Show next level button only for Level 1
            if (nextLevelButton != null)
            {
                nextLevelButton.gameObject.SetActive(currentLevel == 1);
            }

            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("Success Panel is NULL!");
        }

        // Show cursor for clicking buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CheckGameOver()
    {
        // FIXED: Stop game
        gameIsActive = false;
        Time.timeScale = 0f; // Pause game

        Debug.Log("CheckGameOver - Score: " + score + " / Min: " + minimumScoreToWin);

        bool hasEnoughScore = score >= minimumScoreToWin;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (gameOverMessageText != null)
            {
                if (hasEnoughScore)
                {
                    gameOverMessageText.text = "Time's Up!\n" +
                                              "But you collected enough!\n" +
                                              "Score: " + score + " (Min: " + minimumScoreToWin + ")";
                    gameOverMessageText.color = Color.yellow;
                    
                    // Can go to next level
                    if (currentLevel == 1 && nextLevelButton != null)
                    {
                        nextLevelButton.gameObject.SetActive(true);
                    }
                }
                else
                {
                    gameOverMessageText.text = "Time's Up!\n" +
                                              "Not enough score!\n" +
                                              "Score: " + score + " (Need: " + minimumScoreToWin + ")";
                    gameOverMessageText.color = Color.red;
                    
                    // Only restart available
                    if (nextLevelButton != null)
                    {
                        nextLevelButton.gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Game Over Panel is NULL!");
        }

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // FIXED: Button functions with proper time scale reset
    public void RestartLevel()
    {
        Debug.Log("Restart button clicked");
        Time.timeScale = 1f; // IMPORTANT: Resume time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        Debug.Log("Next Level button clicked");
        Time.timeScale = 1f; // IMPORTANT: Resume time
        
        if (currentLevel == 1)
        {
            if (!string.IsNullOrEmpty(level2SceneName))
            {
                SceneManager.LoadScene(level2SceneName);
            }
            else
            {
                Debug.LogError("Level 2 scene name not set!");
            }
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit button clicked");
        Time.timeScale = 1f;
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // FUNGSI BARU UNTUK FIX BUG MENANG OTOMATIS
    public void SetTotalCollectibles(int amount)
    {
        totalCollectibles = amount;
        collectedCount = 0;
        Debug.Log("GameManager: Total crystals set to " + totalCollectibles);
    }
}