using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    public List<QuestionAndAnswer> QnA;
    public GameObject[] options;
    public int currentQuestion;

    public TMP_Text textMesh;

    private void Start()
    {
        genrateQuestion();
    }

    public void correct()
    {
        QnA.RemoveAt(currentQuestion);

        genrateQuestion();
    }

    void setAnswers()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<AnswerScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<TMP_Text>().text = QnA[currentQuestion].Anwers[i];

            if (QnA[currentQuestion].CorrectAnswer == i+1)
            {
                options[i].GetComponent<AnswerScript>().isCorrect = true;

            }
        }
    }

    void genrateQuestion()
    {
        if (QnA.Count > 0)
        {
            currentQuestion = Random.Range(0,QnA.Count);

        textMesh.text = QnA[currentQuestion].Question;
        setAnswers();
        }
        else 
        {
            Debug.Log("no more question");
        }
        

    }
}
