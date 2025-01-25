using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour 
{
    [Serializable]
    public class GameButton
    {
        public Button button;
        public GameObject lockIcon;
        public GameObject tickMark;
        public string sceneName;
        public bool completed;
    }

    public GameButton[] gameButtons;
    public GameObject endGameScreen;
    
    public int currentGameIndex = 0;
    private static MenuManager instance;

    public static MenuManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            ResetProgress();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy(gameObject);
            return;
        }

        // Load saved progress
        currentGameIndex = PlayerPrefs.GetInt("CurrentGameIndex", 0);
        InitializeUI();
        endGameScreen.SetActive(false);
    }

    void InitializeUI()
    {
        
        for (int i = 0; i < gameButtons.Length; i++)
        {
            int index = i;
            if (gameButtons[i].button != null)
            {
                gameButtons[i].button.onClick.RemoveAllListeners();
                gameButtons[i].button.onClick.AddListener(() => LoadGame(index));
            }
            UpdateButtonState(i);
        }

         
    }

    void UpdateButtonState(int buttonIndex)
    {
        // Debug.Log("UpdateButtonState " + buttonIndex) ;
        if (buttonIndex < gameButtons.Length && 
            gameButtons[buttonIndex].lockIcon != null && 
            gameButtons[buttonIndex].tickMark != null &&
            gameButtons[buttonIndex].button != null)
        {
            bool isCompleted = PlayerPrefs.GetInt($"Game{buttonIndex}Completed", 0) == 1;
            bool isUnlocked = buttonIndex <= currentGameIndex;
            // Debug.Log("isCompleted " + isCompleted) ;
            // Debug.Log("currentGameIndex " + currentGameIndex) ;

            gameButtons[buttonIndex].lockIcon.SetActive(!isUnlocked);
            gameButtons[buttonIndex].tickMark.SetActive(isCompleted);
            gameButtons[buttonIndex].button.interactable = isUnlocked;
        }
    }

    void LoadGame(int gameIndex)
    {
        if (gameIndex <= currentGameIndex)
        {
            SceneManager.LoadScene(gameButtons[gameIndex].sceneName);
        }
    }

    public void CompleteCurrentGame()
{
    PlayerPrefs.SetInt($"Game{currentGameIndex}Completed", 1);
    currentGameIndex++;
    PlayerPrefs.SetInt("CurrentGameIndex", currentGameIndex);
    PlayerPrefs.Save();

    if (currentGameIndex >= gameButtons.Length-1) 
    {
        ShowEndGameScreen();
    }
    else 
    {
        SceneManager.LoadScene("Scenes/Progress");
    }
}

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        currentGameIndex = PlayerPrefs.GetInt("CurrentGameIndex", 0);
        // Debug.Log("OnEnable");
        InitializeUI();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Scenes/HomeScreen")
        {
            InitializeUI();
        }
    }

    public void ResetProgress()
    {
        // Debug.Log("Reset");
        currentGameIndex = 0;
        PlayerPrefs.SetInt("CurrentGameIndex", 0);
        for (int i = 0; i < gameButtons.Length; i++)
        {
            PlayerPrefs.DeleteKey($"Game{i}Completed");
        }
        PlayerPrefs.Save();
        InitializeUI();
        
        if (endGameScreen != null)
            endGameScreen.SetActive(false);
    }

    void ShowEndGameScreen()
    {
        if (endGameScreen != null)
            endGameScreen.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    
}