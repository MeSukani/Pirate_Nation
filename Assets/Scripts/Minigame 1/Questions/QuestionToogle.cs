using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionToggle : MonoBehaviour
{
    [SerializeField] private GameObject howToPlayScreen;
    

    public void Trigger()
    {
        if (howToPlayScreen.activeInHierarchy == false)
        {
            howToPlayScreen.SetActive(true);
        }
        else
        {
            howToPlayScreen.SetActive(false);
        }
    }
}
