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
    public GameObject redirectPanel;    // New panel for redirect message
    public TMP_Text redirectText;          // Text component inside redirect panel

    [Header("Progress UI")]
    public TMP_Text progressText;  // Reference to your progress text

    private int currentScore = 0;
    private int movesLeft;
    private bool isTransitioning = false;

    void Start()
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
        if (GameProgress.Instance != null && progressText != null)
        {
            GameProgress.Instance.SetProgressText(progressText);
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();
    }

    public void DecrementMoves()
    {
        movesLeft--;
        UpdateUI();

        if (movesLeft <= 0 && !isTransitioning)
        {
            ShowGameOver();
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
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + currentScore.ToString();
            }
            StartCoroutine(TransitionToFirstScene());
        }
    }

    private IEnumerator TransitionToFirstScene()
    {
        isTransitioning = true;

        // Update game progress
        if (GameProgress.Instance != null)
    {
        GameProgress.Instance.CompleteGame();
        
        // Check if all games are completed
        if (GameProgress.Instance.AreAllGamesCompleted())
        {
            // Load final completion scene or show final message
            SceneManager.LoadScene("FinalScene");
        }
        else
        {
            // Return to AR scene
            SceneManager.LoadScene(1);
        }
    }

        // Show game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + currentScore.ToString();
            }
        }

        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // Hide game over panel and show redirect panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (redirectPanel != null)
        {
            redirectPanel.SetActive(true);
            
            for (int i = 5; i > 0; i--)
            {
                if (redirectText != null)
                {
                    redirectText.text = $"You will be redirected to your next game in {i} seconds...";
                }
                yield return new WaitForSeconds(1f);
            }
        }

        // Load the menu scene
        SceneManager.LoadScene(1);
    }

    public bool HasMovesLeft()
    {
        return movesLeft > 0;
    }
}