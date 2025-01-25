using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;


public class MainGameManager : MonoBehaviour
{
    

    public static MainGameManager Instance { get; private set; }
    
    [Header("UI References")]
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private GameObject questionUI;
    
    [Header("Game State")]
    [SerializeField] private int currentGameProgress = 0;
    [SerializeField] private int totalMaps = 3;
    
    private bool canInteractWithMap = true;
    private Camera arCamera;
    private ARSession arSession;

      private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // Store AR components reference
            var arSessionObject = GameObject.Find("AR Session");
            if (arSessionObject != null)
            {
                arSession = arSessionObject.GetComponent<ARSession>();
                DontDestroyOnLoad(arSessionObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void CloseQuestionUI()
    {
        questionUI.SetActive(false);
        canInteractWithMap = true;  // Re-enable map interaction
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1) // AR scene
        {
            // Ensure we don't create duplicate AR Sessions
            var existingARSessions = FindObjectsOfType<ARSession>();
            foreach (var session in existingARSessions)
            {
                if (session != arSession)
                {
                    Destroy(session.gameObject);
                }
            }

            if (arSession != null)
            {
                arSession.enabled = true;
            }
            
            UpdateProgressUI();
            canInteractWithMap = true;
            if (questionUI != null)
            {
                questionUI.SetActive(false);
            }
        }
        else
        {
            if (arSession != null)
            {
                arSession.enabled = false;
            }
        }
    }

    public void UpdateProgressUI()
    {
        if (progressText != null)
        {
            progressText.text = $"{currentGameProgress}/{totalMaps}";
        }
    }

    public void OnMapFound()
    {
        if (!canInteractWithMap) return;
        
        // Show riddle UI
        if (questionUI != null)
        {
            questionUI.SetActive(true);
            canInteractWithMap = false;
        }
    }
    

    public void OnRiddleSolved()
    {
        currentGameProgress++;
        UpdateProgressUI();
        
        // Load appropriate minigame based on progress
        switch (currentGameProgress)
        {
            case 1:
                SceneManager.LoadScene(2,LoadSceneMode.Single); // Replace with your actual scene name
                break;
            case 2:
                SceneManager.LoadScene(3, LoadSceneMode.Single);
                break;
            case 3:
                SceneManager.LoadScene(4, LoadSceneMode.Single);
                break;
        }
    }

    public void OnMinigameCompleted()
    {
        if (currentGameProgress < totalMaps)
        {
            // Return to AR scene to find next map
            SceneManager.LoadScene(1); // AR scene
        }
        else
        {
            // Game completed
            // Add your game completion logic here
            Debug.Log("Game Completed!");
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}