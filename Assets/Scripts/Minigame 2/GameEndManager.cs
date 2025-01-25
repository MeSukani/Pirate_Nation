using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEndManager : MonoBehaviour
{
   public GameObject endScreen;
   public GameObject redirectPanel;
   public TMP_Text redirectText;
   private bool isTransitioning = false;

   void Start() 
   {
        endScreen.SetActive(false);
        redirectPanel.SetActive(false);
   }

   public void TriggerEndSequence()
   {
       if (!isTransitioning)
       {
           StartCoroutine(ShowEndScreen());
       }
   }

   private IEnumerator ShowEndScreen()
   {
       isTransitioning = true;
       endScreen.SetActive(true);
       yield return new WaitForSeconds(2f);
       endScreen.SetActive(false);
       redirectPanel.SetActive(true);
       
       for (int i = 5; i > 0; i--)
       {
           redirectText.text = $"You will be redirected to your next game in {i} seconds...";
           yield return new WaitForSeconds(1f);
       }
       
       MenuManager.Instance.CompleteCurrentGame();
       yield return null;
       SceneManager.LoadScene("Scenes/Progress");
   }
}