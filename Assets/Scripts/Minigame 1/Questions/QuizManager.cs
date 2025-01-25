using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class QuizManager : MonoBehaviour
{
    public List<QuestionAndAnswer> QnA;
    public GameObject[] options;
    public int currentQuestion;
    public TMP_Text questionText;    // Renamed from textMesh for clarity
    public TMP_Text feedbackText;    // New text element for feedback

    private void Start()
    {
        generateQuestion();
        // Hide feedback text initially
        feedbackText.gameObject.SetActive(false);
    }

    public void correct(GameObject selectedButton)
    {
        // Disable all buttons during the delay
        foreach (GameObject option in options)
        {
            option.GetComponent<Button>().interactable = false;
        }

        StartCoroutine(ProcessAnswer(selectedButton, true));
    }

    public void wrong(GameObject selectedButton)
    {
        StartCoroutine(ProcessAnswer(selectedButton, false));
    }

    IEnumerator ProcessAnswer(GameObject selectedButton, bool isCorrect)
    {
        Image buttonImage = selectedButton.GetComponent<Image>();
        Color originalColor = buttonImage.color;

        // Change button color based on correctness
        buttonImage.color = isCorrect ? Color.green : Color.red;

        // Show feedback text
        feedbackText.gameObject.SetActive(true);
        feedbackText.text = isCorrect ? "Correct!" : "Incorrect";
        feedbackText.color = isCorrect ? Color.green : Color.red;

        if (isCorrect)
        {
            yield return new WaitForSeconds(3f);
            QnA.RemoveAt(currentQuestion);
            MainGameManager.Instance.OnRiddleSolved();
        }
        else
        {
            yield return new WaitForSeconds(1f);
            buttonImage.color = originalColor;
            feedbackText.gameObject.SetActive(false);
            
            // Re-enable all buttons after showing incorrect feedback
            foreach (GameObject option in options)
            {
                option.GetComponent<Button>().interactable = true;
            }
        }
    }

    void setAnswers()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<AnswerScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<TMP_Text>().text = QnA[currentQuestion].Anwers[i];
            if (QnA[currentQuestion].CorrectAnswer == i + 1)
            {
                options[i].GetComponent<AnswerScript>().isCorrect = true;
            }
        }
    }

    void generateQuestion()
    {
        if (QnA.Count > 0)
        {
            currentQuestion = Random.Range(0, QnA.Count);
            questionText.text = QnA[currentQuestion].Question;
            feedbackText.gameObject.SetActive(false);  // Hide feedback when new question is generated
            setAnswers();
        }
        else
        {
            Debug.Log("No more questions");
        }
    }
}