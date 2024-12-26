using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    private float speed;
    private float destroyDistance = -5f;

    public void Initialize(float moveSpeed)
    {
        speed = moveSpeed;
    }

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);
        
        if (transform.position.z < destroyDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Handle collision with player (game over)
            GameManager.Instance.GameOver();
        }
    }
}