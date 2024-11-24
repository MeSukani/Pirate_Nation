using UnityEngine;
using UnityEngine.SceneManagement;

public class ARGameStarter : MonoBehaviour
{
    // Assign these in the inspector
    public int gameSceneIndex = 2;  // The scene index of your game
    public int gameIndex;           // Which game this marker represents (0, 1, or 2)

    public void OnMarkerTapped()
    {
        // Set which game is being played before loading it
        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.SetCurrentGame(gameIndex);
        }
        
        // Load the game scene
        SceneManager.LoadScene(gameSceneIndex);
    }
}