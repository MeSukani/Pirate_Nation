using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void backButton()
    {
        SceneManager.LoadSceneAsync(0);
    }
     public void QuitGame()
   {
        Application.Quit();
   }
}
