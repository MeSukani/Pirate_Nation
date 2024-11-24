using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameProgress : MonoBehaviour
{
    private static GameProgress instance;
    public static GameProgress Instance
    {
        get { return instance; }
    }

    private TMP_Text progressText;
    private HashSet<int> completedGames = new HashSet<int>();
    private const string PROGRESS_KEY = "CompletedGames";
    public int totalGames = 3;

    // Add this to identify which game is being played
    private int currentGameIndex = -1;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadProgress()
    {
        completedGames.Clear();
        string savedGames = PlayerPrefs.GetString(PROGRESS_KEY, "");
        if (!string.IsNullOrEmpty(savedGames))
        {
            string[] games = savedGames.Split(',');
            foreach (string game in games)
            {
                if (int.TryParse(game, out int gameIndex))
                {
                    completedGames.Add(gameIndex);
                }
            }
        }
        UpdateProgressText();
    }

    private void SaveProgress()
    {
        string savedGames = string.Join(",", completedGames);
        PlayerPrefs.SetString(PROGRESS_KEY, savedGames);
        PlayerPrefs.Save();
    }

    // Call this when starting a specific game (from your AR scene)
    public void SetCurrentGame(int gameIndex)
    {
        currentGameIndex = gameIndex;
        Debug.Log($"Starting Game {gameIndex + 1}");
    }

    // Call this when a game is completed
    public void CompleteGame()
    {
        if (currentGameIndex >= 0 && currentGameIndex < totalGames)
        {
            if (!completedGames.Contains(currentGameIndex))
            {
                completedGames.Add(currentGameIndex);
                SaveProgress();
                UpdateProgressText();
                Debug.Log($"Completed Game {currentGameIndex + 1}. Total completed: {completedGames.Count}");
            }
            else
            {
                Debug.Log($"Game {currentGameIndex + 1} was already completed!");
            }
        }
        else
        {
            Debug.LogWarning("Trying to complete a game but no valid game index is set!");
        }
    }

    public void SetProgressText(TMP_Text newProgressText)
    {
        progressText = newProgressText;
        UpdateProgressText();
    }

    private void UpdateProgressText()
    {
        if (progressText != null)
        {
            progressText.text = $"{completedGames.Count}/{totalGames}";
        }
    }

    public bool IsGameCompleted(int gameIndex)
    {
        return completedGames.Contains(gameIndex);
    }

    public void ResetProgress()
    {
        completedGames.Clear();
        SaveProgress();
        UpdateProgressText();
        Debug.Log("Progress Reset");
    }

    // Call this to check if all games are completed
    public bool AreAllGamesCompleted()
    {
        return completedGames.Count >= totalGames;
    }
}