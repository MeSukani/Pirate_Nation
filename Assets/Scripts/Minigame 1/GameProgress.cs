using UnityEngine;
using TMPro;

public class GameProgress : MonoBehaviour
{
    private static GameProgress instance;
    public static GameProgress Instance
    {
        get { return instance; }
    }

    private TMP_Text progressText;
    private int gamesCompleted = 0;
    private const string PROGRESS_KEY = "GamesCompleted";
    public int totalGames = 3;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            // Load saved progress when game starts
            gamesCompleted = PlayerPrefs.GetInt(PROGRESS_KEY, 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CompleteGame()
    {
        gamesCompleted = Mathf.Min(gamesCompleted + 1, totalGames);
        PlayerPrefs.SetInt(PROGRESS_KEY, gamesCompleted);
        PlayerPrefs.Save();
        UpdateProgressText();
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
            progressText.text = $"{gamesCompleted}/{totalGames}";
        }
    }

    public int GetCompletedGames()
    {
        return gamesCompleted;
    }

    public void ResetProgress()
    {
        gamesCompleted = 0;
        PlayerPrefs.SetInt(PROGRESS_KEY, 0);
        PlayerPrefs.Save();
        UpdateProgressText();
    }
}