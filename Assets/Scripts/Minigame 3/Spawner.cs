using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float life = 50;
    

    void Update()
    {
        
        
            life -= Time.deltaTime;
            if (life <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                transform.Translate(0, 0, -20 * Time.deltaTime);
            }
        
       
        
    }
}
