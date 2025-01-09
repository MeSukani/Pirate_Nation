using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    public GameObject tapToStartUI;

    void Start()
    {
        Time.timeScale = 0f;
       
    }

    void Update()
    {
        if (tapToStartUI.activeSelf && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }

    void StartGame()
    {
        tapToStartUI.SetActive(false);
        Time.timeScale = 1f;
    }


    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}