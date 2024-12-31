using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShipController : MonoBehaviour 
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float boundaryLimit = 2f;
    [SerializeField] private float tiltSpeed = 5f;
    [SerializeField] private float maxTiltAngle = 10f;
    [SerializeField] private float returnSpeed = 3f;
    
    private Vector2 touchStartPos;
    private bool isTouching = false;
    private float currentTiltAngle = 0f;
    private const float baseRotation = -90f;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isTouching = true;
                    break;

                case TouchPhase.Moved:
                    if (isTouching)
                    {
                        float deltaX = (touch.position.x - touchStartPos.x) * moveSpeed * Time.deltaTime;
                        Vector3 newPosition = transform.position + new Vector3(deltaX, 0, 0);
                        
                        // Position clamping
                        newPosition.x = Mathf.Clamp(newPosition.x, -boundaryLimit, boundaryLimit);
                        transform.position = newPosition;
                        
                        // Calculate tilt
                        float targetTilt = 0f;
                        if (deltaX > 0) // Moving right
                        {
                            targetTilt = -maxTiltAngle; // Positive for right tilt
                        }
                        else if (deltaX < 0) // Moving left
                        {
                            targetTilt = maxTiltAngle; // Negative for left tilt
                        }
                        
                        // Smooth tilt transition
                        currentTiltAngle = Mathf.Lerp(currentTiltAngle, targetTilt, Time.deltaTime * tiltSpeed);
                        
                        touchStartPos = touch.position;
                    }
                    break;

                case TouchPhase.Ended:
                    isTouching = false;
                    break;
            }
        }
        else
        {
            // Return to center when not touching
            currentTiltAngle = Mathf.Lerp(currentTiltAngle, 0f, Time.deltaTime * returnSpeed);
        }

        // Apply rotation
        transform.eulerAngles = new Vector3(baseRotation + currentTiltAngle, -90f, 0f);
    }
}