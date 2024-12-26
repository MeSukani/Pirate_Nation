using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
    [SerializeField] private float tiltSpeed = 10f;
    [SerializeField] private float maxMovementRange = 2f;
    [SerializeField] private float tiltSensitivity = 3f;
    [SerializeField] private float smoothing = 2f;
    
    private Vector3 currentVelocity;
    private float targetX;
    private Vector3 calibratedZero;
    private bool isCalibrated = false;

    // Reference to the Input Action asset
    private PlayerInput playerInput;
    private InputAction tiltAction;

    private void Awake()
    {
        // Set up input
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            playerInput = gameObject.AddComponent<PlayerInput>();
        }

        // Enable accelerometer
        InputSystem.EnableDevice(Accelerometer.current);
    }

    private void OnEnable()
    {
        // Check if accelerometer is available
        if (Accelerometer.current != null)
        {
            CalibrateCenter();
        }
        else
        {
            Debug.LogWarning("Accelerometer not found on device!");
        }
    }

    private void OnDisable()
    {
        InputSystem.DisableDevice(Accelerometer.current);
    }

    public void CalibrateCenter()
    {
        if (Accelerometer.current != null)
        {
            calibratedZero = Accelerometer.current.acceleration.ReadValue();
            isCalibrated = true;
            ResetPosition();
        }
    }

    private void Update()
    {
        if (!isCalibrated || Accelerometer.current == null) return;

        // Get accelerometer data
        Vector3 acceleration = Accelerometer.current.acceleration.ReadValue();
        Vector3 tiltValue = acceleration - calibratedZero;
        float tilt = tiltValue.x * tiltSensitivity;

        // Calculate target position
        targetX += tilt * tiltSpeed * Time.deltaTime;
        targetX = Mathf.Clamp(targetX, -maxMovementRange, maxMovementRange);

        // Apply smooth movement
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.SmoothDamp(
            transform.position.x,
            targetX,
            ref currentVelocity.x,
            1f / smoothing
        );

        transform.position = newPosition;
    }

    public void ResetPosition()
    {
        targetX = 0f;
        currentVelocity = Vector3.zero;
        Vector3 resetPos = transform.position;
        resetPos.x = 0f;
        transform.position = resetPos;
    }

    // Optional: Debug GUI
    private void OnGUI()
    {
        if (Debug.isDebugBuild && Accelerometer.current != null)
        {
            GUILayout.Label($"Tilt: {Accelerometer.current.acceleration.ReadValue().x:F2}");
            GUILayout.Label($"Position: {transform.position.x:F2}");
            
            if (GUILayout.Button("Calibrate"))
            {
                CalibrateCenter();
            }
        }
    }
}