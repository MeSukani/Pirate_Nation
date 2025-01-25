using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class QuestionUI : MonoBehaviour
{
     public GameObject questionUI;
     
    private void Start()
    {
        questionUI.SetActive(false);
    }

    private void Update()
    {
        if ( Input.touchCount == 0) return;
        
        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;
        
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
           
           questionUI.SetActive(true);
        }
    }
    

    
}
