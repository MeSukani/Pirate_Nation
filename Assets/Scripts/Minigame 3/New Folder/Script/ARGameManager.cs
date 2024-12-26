using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARGameManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject shipPrefab;
    [SerializeField] private GameObject waterPlanePrefab;
    
    private GameObject spawnedShip;
    private GameObject spawnedWater;
    private bool isGameStarted = false;

    void Update()
    {
        if (!isGameStarted && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                var hits = new List<ARRaycastHit>();
                if (raycastManager.Raycast(touch.position, hits, TrackableType.Planes))
                {
                    Pose hitPose = hits[0].pose;
                    SpawnGameElements(hitPose);
                    isGameStarted = true;
                }
            }
        }
    }

    private void SpawnGameElements(Pose pose)
    {
        spawnedWater = Instantiate(waterPlanePrefab, pose.position, pose.rotation);
        Vector3 shipPosition = pose.position + Vector3.up * 0.1f; // Slight offset above water
        spawnedShip = Instantiate(shipPrefab, shipPosition, pose.rotation);
    }
}