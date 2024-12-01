using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnswerScript : MonoBehaviour
{
public bool isCorrect = false;
public QuizManager quizManager;
   public void Answer()
   {
        if (isCorrect)
        {
            Debug.Log("Correct");
            quizManager.correct();
            SceneManager.LoadScene(2);
            
        }
        else
        {
            Debug.Log("incorrct");
            quizManager.correct();
        }
   }
}
