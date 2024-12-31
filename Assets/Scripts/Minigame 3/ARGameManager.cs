using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARGameManager : MonoBehaviour
{
   public static ARGameManager Instance;
   public bool isGameStarted = false;
   public bool isGameOver = false;
   public GameObject startScreen;
   public GameObject gameOverScreen;

   private void Awake() 
   {
        if (Instance == null)
        {
            Instance = this;
        } 
        else Destroy(gameObject);
   }

   public void startGame()
   {
    isGameStarted = true;
    isGameOver = false;
   }

   public void gameOver()
   {
    isGameStarted = false;
    isGameOver = true;
   }

   void Update() 
   {
        if (isGameStarted == true)
        {
            startScreen.SetActive(false);
        }
        if (isGameOver == true)
        {
            gameOverScreen.SetActive(true);
            
        }
   }

   
}