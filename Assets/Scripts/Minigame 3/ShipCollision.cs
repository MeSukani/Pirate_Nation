using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShipCollision : MonoBehaviour
{
   public GameObject deathExplotion;
   // public GameObject checkpointSound;
    public TMP_Text scoreText;
    public int score;
    public GameObject gameoverUI;

    private void Start()
    {
        gameoverUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other) 
    {
        gameoverUI.SetActive(false);
        if (other.gameObject.tag == "Obstacle")
        {
          GameObject deathObj = Instantiate(deathExplotion, transform.position, transform.rotation) as GameObject;
            
            ARGameManager.Instance.gameOver();
            gameoverUI.SetActive(true);
            Time.timeScale = 0f;
        }
        else if (other.gameObject.tag == "Checkpoint")
        {
            score += 1;
            scoreText.text = score.ToString();
           // Instantiate(checkpointSound, transform.position, transform.rotation);
        }

    }
}
