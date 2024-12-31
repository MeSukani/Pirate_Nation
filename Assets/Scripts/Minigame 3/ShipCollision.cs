using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShipCollision : MonoBehaviour
{
   //public GameObject deathExplotion;
   // public GameObject checkpointSound;
    public TMP_Text scoreText;
    public int score;

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Obstacle")
        {
          // GameObject deathObj = Instantiate(deathExplotion, transform.position, transform.rotation) as GameObject;
            
            ARGameManager.Instance.gameOver();
        }
        else if (other.gameObject.tag == "Checkpoint")
        {
            score += 1;
            scoreText.text = score.ToString();
           // Instantiate(checkpointSound, transform.position, transform.rotation);
        }

    }
}
