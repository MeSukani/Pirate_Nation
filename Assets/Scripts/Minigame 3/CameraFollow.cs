using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private float rotationSpeed = 5f;
    
    private Vector3 _offset;
    private Vector3 _currentVelocity = Vector3.zero;
    private Quaternion _targetRotation;

    private void Awake() 
    {    
        _offset = transform.position - target.position;
        _targetRotation = transform.rotation;
    }

    private void LateUpdate() 
    {
        // Position following
        Vector3 targetPosition = target.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime);

        // Rotation following
        Quaternion targetRotation = Quaternion.Euler(
            1f, // Tilt with ship
            1f,                 // Keep Y at -90
            target.eulerAngles.x                    // Keep Z at 0
        );

        // Smoothly interpolate rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}