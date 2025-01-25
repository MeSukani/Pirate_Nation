using UnityEngine;
using TMPro;
using System.Collections;

public class ShipCollision : MonoBehaviour 
{
    public GameObject deathExplotion;
    public TMP_Text scoreText;
    public int score;
    public GameObject gameoverUI;
    public GameObject winScreen; // Add win screen reference
    public AudioSource collisionSound;
    public AudioSource checkpointSound;
    

    private void Start()
    {
        gameoverUI.SetActive(false);
        winScreen.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            if (collisionSound != null) collisionSound.Play();
            GameObject deathObj = Instantiate(deathExplotion, transform.position, transform.rotation);
            ARGameManager.Instance.gameOver();
            gameoverUI.SetActive(true);
            Time.timeScale = 0f;
        }
        else if (other.gameObject.CompareTag("Checkpoint"))
        {
            if (checkpointSound != null) checkpointSound.Play();
            score += 1;
            scoreText.text = score.ToString();
        }
        else if (other.gameObject.CompareTag("Treasure"))
        {
            
            winScreen.SetActive(true);
            StartCoroutine(WinSequence());
        }
    }

    IEnumerator WinSequence()
    {
        yield return new WaitForSeconds(2f);
        MenuManager.Instance.CompleteCurrentGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Progress");
    }
}
