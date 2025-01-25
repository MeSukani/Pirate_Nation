using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameUIManager : MonoBehaviour
{
    [Header("Game UI")]
    public TMP_Text scoreText;
    public TMP_Text movesText;
    public int totalMoves = 30;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;

    [Header("Redirect UI")]
    public GameObject redirectPanel;
    public TMP_Text redirectText;

    [Header("Progress UI")]
    public TMP_Text progressText;

    [Header("Settings")]
    public float gameOverDelay = 2f;
    public float redirectCountdown = 5f;
    public int menuSceneIndex = 1;  // Make scene index configurable

    private int currentScore = 0;
    private int movesLeft;
    private bool isTransitioning = false;

    void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        movesLeft = totalMoves;
        UpdateUI();
        
        // Initialize UI states
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (redirectPanel != null)
        {
            redirectPanel.SetActive(false);
        }
        
        // Initialize progress
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        if (GameProgress.Instance != null && progressText != null)
        {
            GameProgress.Instance.SetProgressText(progressText);
        }
    }

    public void AddScore(int points)
    {
        if (!isTransitioning)
        {
            currentScore += points;
            UpdateUI();
        }
    }

    public void DecrementMoves()
    {
        if (!isTransitioning)
        {
            movesLeft--;
            UpdateUI();

            if (movesLeft <= 0)
            {
                ShowGameOver();
            }
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
        if (movesText != null)
        {
            movesText.text = movesLeft.ToString();
        }
    }

    private void ShowGameOver()
    {
        if (!isTransitioning && gameOverPanel != null)
        {
            isTransitioning = true;
            gameOverPanel.SetActive(true);
            
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Final Score: {currentScore}";
            }
            
            StartCoroutine(TransitionToFirstScene());
        }
    }

    private IEnumerator TransitionToFirstScene()
{
    yield return new WaitForSeconds(gameOverDelay);

    if (gameOverPanel != null)
        gameOverPanel.SetActive(false);
        
    if (redirectPanel != null)
    {
        redirectPanel.SetActive(true);
        
        for (int i = (int)redirectCountdown; i > 0; i--)
        {
            if (redirectText != null)
                redirectText.text = $"You will be redirected to your next game in {i} seconds...";
            yield return new WaitForSeconds(1f);
        }
    }

    // Mark current game as complete first
    MenuManager.Instance.CompleteCurrentGame();
    
    // Wait one frame to ensure PlayerPrefs are saved
    yield return null;
    
    // Now load the menu scene
    SceneManager.LoadScene("Scenes/Progress");
}

    public bool HasMovesLeft()
    {
        return movesLeft > 0 && !isTransitioning;
    }

   
}