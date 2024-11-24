using UnityEngine;
using TMPro;

public class GameProgressManager : MonoBehaviour
{
    public TMP_Text progressText;
    public int totalGames = 3;

    void Start()
    {
        UpdateProgressText();
    }

    void OnEnable()
    {
        UpdateProgressText();
    }

    private void UpdateProgressText()
    {
        if (progressText != null)
        {
            int completed = PlayerPrefs.GetInt("GamesCompleted", 0);
            progressText.text = $"{completed}/{totalGames}";
        }
    }
}
