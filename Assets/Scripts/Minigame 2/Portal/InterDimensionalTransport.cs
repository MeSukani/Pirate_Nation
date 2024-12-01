using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InterDimensionalTransport : MonoBehaviour
{

    public Material[] materials;

    void Start()
    {
        foreach (var mat in materials)
            {
                mat.SetInt("_StencilTest", (int)CompareFunction.Equal);
            }
    }

    void OnTriggerStay(Collider other) 
    {
        if (other.name != "Main Camera")
        {
            return;
        }

        //Outside of other World
        if (transform.position.z > other.transform.position.z)
        {
            foreach (var mat in materials)
            {
                mat.SetInt("_StencilTest", (int)CompareFunction.Equal);
            }
            
        }
        //Inside other dimension
        else
        {
            foreach (var mat in materials)
            {
                mat.SetInt("_StencilTest", (int)CompareFunction.NotEqual);

            }
        }
    }

    void OnDestroy() 
    {
        foreach (var mat in materials)
                    {
                        mat.SetInt("_StencilTest", (int)CompareFunction.NotEqual);

                    }  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
