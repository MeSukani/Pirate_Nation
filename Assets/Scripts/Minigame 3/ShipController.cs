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
    [SerializeField] private float wheelRotationSpeed = 90f; // Control wheel rotation speed
    [SerializeField] private Transform shipWheel; // Reference to the wheel object

    [Header("Audio")]
   public AudioSource waterAmbientSound;
   public AudioSource shipTiltSound;
   public AudioSource wheelRotationSound;
    
    private Vector2 touchStartPos;
    private bool isTouching = false;
    private float currentTiltAngle = 0f;
    private float currentWheelRotation = 0f;
    private const float baseRotation = 0.1f;

     void Start()
   {
       if (waterAmbientSound != null)
       {
           waterAmbientSound.Play();
           waterAmbientSound.loop = true;
       }
   }

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
                        float targetWheelRotation = 0f;

                        if (deltaX > 0) // Moving right
                        {
                            targetTilt = -maxTiltAngle;
                            targetWheelRotation = 360f; // Rotate wheel clockwise
                        }
                        else if (deltaX < 0) // Moving left
                        {
                            targetTilt = maxTiltAngle;
                            targetWheelRotation = -360f; // Rotate wheel counter-clockwise
                        }
                        
                        // Smooth tilt transition for ship
                        currentTiltAngle = Mathf.Lerp(currentTiltAngle, targetTilt, Time.deltaTime * tiltSpeed);
                        
                        // Smooth rotation for wheel
                        currentWheelRotation = Mathf.Lerp(currentWheelRotation, targetWheelRotation, 
                            Time.deltaTime * wheelRotationSpeed);
                        
                        touchStartPos = touch.position;
                         if (shipTiltSound != null && Mathf.Abs(deltaX) > 0.01f)
                        {
                            if (!shipTiltSound.isPlaying) shipTiltSound.Play();
                        }

                        // Play wheel rotation sound
                        if (wheelRotationSound != null && Mathf.Abs(deltaX) > 0.01f)
                        {
                            if (!wheelRotationSound.isPlaying) wheelRotationSound.Play();
                        }
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
            currentWheelRotation = Mathf.Lerp(currentWheelRotation, 0f, Time.deltaTime * returnSpeed);
            if (shipTiltSound != null && shipTiltSound.isPlaying) shipTiltSound.Stop();
            if (wheelRotationSound != null && wheelRotationSound.isPlaying) wheelRotationSound.Stop();
       
        }

        // Apply ship rotation
        transform.eulerAngles = new Vector3(baseRotation + currentTiltAngle, -90f, 0f);

        // Apply wheel rotation
        if (shipWheel != null)
        {
            shipWheel.localRotation = Quaternion.Euler(currentWheelRotation, shipWheel.localEulerAngles.y, 
                shipWheel.localEulerAngles.z);
        }
    }
}