using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class infoButton : MonoBehaviour
{
    [SerializeField] private GameObject howToPlayScreen;

    private void Start() 
    {
        howToPlayScreen.SetActive(true);      
    }
    

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
