using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectManager : MonoBehaviour 
{
    public static SceneObjectManager Instance;
    public GameObject dimension;  // Drag Dimension object here
    public GameObject quad;       // Drag Quad object here
    
    void Awake()
    {
        Instance = this;
        // Start with both objects disabled
        dimension.SetActive(false);
        quad.SetActive(false);
    }
}