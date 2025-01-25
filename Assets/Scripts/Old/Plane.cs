using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(ARPlaneManager))]
[RequireComponent(typeof(ARRaycastManager))]
public class plane : MonoBehaviour 
{
    [Header("AR Components")]
    private ARPlaneManager planeManager;
    private ARRaycastManager raycastManager;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject[] plants;
    [SerializeField] private GameObject[] stones;
    [SerializeField] private GameObject puzzlePiecePrefab;
    
    [Header("Spawn Settings")]
    [SerializeField] private float growDuration = 2f;
    [SerializeField] private float plantSpacing = 0.5f;
    [SerializeField] private float stoneSpacing = 1f;
    [SerializeField] private int maxPlantsPerPlane = 20;
    [SerializeField] private int maxStonesPerPlane = 2;
    [SerializeField] private int puzzlePiecesPerPlane = 3;
    
    [Header("Puzzle Piece Settings")]
    [SerializeField] private float puzzlePieceMinScale = 0.2f;
    [SerializeField] private float puzzlePieceMaxScale = 0.3f;
    [SerializeField] private float puzzleYOffset = 0.05f;

     
    
    private Dictionary<ARPlane, List<GameObject>> spawnedObjects = new Dictionary<ARPlane, List<GameObject>>();
    private Dictionary<ARPlane, List<GameObject>> spawnedPuzzles = new Dictionary<ARPlane, List<GameObject>>();
    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
    private Camera arCamera;
    
    

    private void Awake()
    {
        // Get required components
        planeManager = GetComponent<ARPlaneManager>();
        raycastManager = GetComponent<ARRaycastManager>();
        arCamera = Camera.main;
        
        // Validate required components
        if (planeManager == null || raycastManager == null || arCamera == null)
        {
            Debug.LogError("Missing required AR components!");
            enabled = false;
            return;
        }
        
        // Validate prefabs
        if (puzzlePiecePrefab == null)
        {
            Debug.LogError("Puzzle piece prefab is not assigned!");
            enabled = false;
            return;
        }
    }
    private void Update()
{
    // Mobile touch input
    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            CheckRaycastHit(touch.position);
        }
    }
    
    // PC mouse input
    if (Input.GetMouseButtonDown(0))
    {
        CheckRaycastHit(Input.mousePosition);
    }
}

private void CheckRaycastHit(Vector2 screenPosition)
{
    Ray ray = Camera.main.ScreenPointToRay(screenPosition);
    RaycastHit hit;
    
    if (Physics.Raycast(ray, out hit))
    {
        Debug.Log("Hit object: " + hit.collider.gameObject.name);
        if (hit.collider.CompareTag("PuzzlePiece"))
        {
            Debug.Log("Map found - attempting to show UI");
            MainGameManager.Instance.OnMapFound();
        }
    }
}
    
    private void OnEnable()
    {
        planeManager.planesChanged += HandlePlanesChanged;
    }
    
    private void OnDisable()
    {
        planeManager.planesChanged -= HandlePlanesChanged;
    }
    
    private void HandlePlanesChanged(ARPlanesChangedEventArgs args)
{
    // For added planes
    foreach (var plane in args.added)
    {
        if (plane != null && !spawnedObjects.ContainsKey(plane))  // Add this check
        {
            spawnedObjects[plane] = new List<GameObject>();
            spawnedPuzzles[plane] = new List<GameObject>();
            SpawnPuzzlePieces(plane);
            SpawnObjects(plane);
        }
    }
    
    // For updated planes
    foreach (var plane in args.updated)
    {
        if (plane != null && spawnedObjects.ContainsKey(plane) && ShouldSpawnMoreObjects(plane))  // Add ContainsKey check
        {
            SpawnObjects(plane);
        }
    }
    
    // For removed planes
    foreach (var plane in args.removed)
    {
        if (spawnedObjects.ContainsKey(plane))  // Add this check
        {
            var objects = spawnedObjects[plane];
            foreach (var obj in objects)
            {
                if (obj != null)
                    Destroy(obj);
            }
            spawnedObjects.Remove(plane);
        }
        
        if (spawnedPuzzles.ContainsKey(plane))  // Add this check
        {
            var puzzles = spawnedPuzzles[plane];
            foreach (var puzzle in puzzles)
            {
                if (puzzle != null)
                    Destroy(puzzle);
            }
            spawnedPuzzles.Remove(plane);
        }
    }
}
    
    private void SpawnPuzzlePieces(ARPlane plane)
    {
         if (plane == null || !spawnedPuzzles.ContainsKey(plane)) return;
        
        for (int i = 0; i < puzzlePiecesPerPlane; i++)
        {
            Vector3 position = GetRandomPlanePosition(plane);
            position.y += puzzleYOffset;
            
            if (IsValidPosition(position, plantSpacing))
            {
                GameObject puzzlePiece = SpawnObject(puzzlePiecePrefab, position, plane);
                if (puzzlePiece != null)
                {
                    spawnedPuzzles[plane].Add(puzzlePiece);
                    StartCoroutine(GrowPuzzlePiece(puzzlePiece));
                }
            }
        }
    }
    
    private IEnumerator GrowPuzzlePiece(GameObject puzzlePiece)
    {
        if (puzzlePiece == null) yield break;
        
        var targetScale = Vector3.one * Random.Range(puzzlePieceMinScale, puzzlePieceMaxScale);
        puzzlePiece.transform.localScale = Vector3.zero;
        
        float elapsed = 0;
        while (elapsed < growDuration && puzzlePiece != null)
        {
            elapsed += Time.deltaTime;
            if (puzzlePiece != null)
            {
                puzzlePiece.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsed / growDuration);
            }
            yield return null;
        }
        
        if (puzzlePiece != null)
        {
            puzzlePiece.transform.localScale = targetScale;
            puzzlePiece.transform.position = new Vector3(
                puzzlePiece.transform.position.x,
                puzzlePiece.transform.position.y + puzzleYOffset,
                puzzlePiece.transform.position.z
            );
        }
    }

    
    
    
    
    private IEnumerator AnimatePuzzlePiece(GameObject puzzlePiece)
    {
        if (puzzlePiece == null) yield break;
        
        Vector3 originalScale = puzzlePiece.transform.localScale;
        Vector3 highlightScale = originalScale * 1.2f;
        float duration = 0.3f;
        
        // Scale up
        float elapsed = 0;
        while (elapsed < duration && puzzlePiece != null)
        {
            elapsed += Time.deltaTime;
            puzzlePiece.transform.localScale = Vector3.Lerp(originalScale, highlightScale, elapsed / duration);
            yield return null;
        }
        
        // Scale back
        elapsed = 0;
        while (elapsed < duration && puzzlePiece != null)
        {
            elapsed += Time.deltaTime;
            puzzlePiece.transform.localScale = Vector3.Lerp(highlightScale, originalScale, elapsed / duration);
            yield return null;
        }
        
        if (puzzlePiece != null)
        {
            puzzlePiece.transform.localScale = originalScale;
        }
    }
    
    private void SpawnObjects(ARPlane plane)
    {
         if (plane == null || !spawnedObjects.ContainsKey(plane)) return;
        if (plants != null && plants.Length > 0)
            StartCoroutine(SpawnObjectsCoroutine(plane, plants, plantSpacing, maxPlantsPerPlane));
        if (stones != null && stones.Length > 0)
            StartCoroutine(SpawnObjectsCoroutine(plane, stones, stoneSpacing, maxStonesPerPlane));
    }
    
    private bool ShouldSpawnMoreObjects(ARPlane plane)
    {
        if (plane == null || !spawnedObjects.ContainsKey(plane)) return false;
        
        float planeArea = plane.size.x * plane.size.y;
        int maxObjects = maxPlantsPerPlane + maxStonesPerPlane;
        int desiredObjects = Mathf.Min(maxObjects, Mathf.FloorToInt(planeArea / (plantSpacing * plantSpacing)));
        
        return spawnedObjects[plane].Count < desiredObjects;
    }
    
    private IEnumerator SpawnObjectsCoroutine(ARPlane plane, GameObject[] prefabs, float spacing, int maxCount)
    {
        if (plane == null || !spawnedObjects.ContainsKey(plane) || prefabs == null || prefabs.Length == 0) 
            yield break;
        
        var currentCount = spawnedObjects[plane].Count;
        while (currentCount < maxCount)
        {
            if (plane == null) yield break;
            
            var position = GetRandomPlanePosition(plane);
            if (IsValidPosition(position, spacing))
            {
                var prefab = prefabs[Random.Range(0, prefabs.Length)];
                var obj = SpawnObject(prefab, position, plane);
                if (obj != null)
                {
                    spawnedObjects[plane].Add(obj);
                    StartCoroutine(GrowObject(obj));
                    currentCount++;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private Vector3 GetRandomPlanePosition(ARPlane plane)
    {
        if (plane == null) return Vector3.zero;
        
        var size = plane.size;
        var localPos = new Vector3(
            Random.Range(-size.x / 2f, size.x / 2f),
            0,
            Random.Range(-size.y / 2f, size.y / 2f)
        );
        return plane.transform.TransformPoint(localPos);
    }
    
    private bool IsValidPosition(Vector3 position, float spacing)
    {
        foreach (var kvp in spawnedObjects)
        {
            if (kvp.Key == null) continue;
            foreach (var obj in kvp.Value)
            {
                if (obj != null && Vector3.Distance(obj.transform.position, position) < spacing)
                    return false;
            }
        }
        
        foreach (var kvp in spawnedPuzzles)
        {
            if (kvp.Key == null) continue;
            foreach (var puzzle in kvp.Value)
            {
                if (puzzle != null && Vector3.Distance(puzzle.transform.position, position) < spacing)
                    return false;
            }
        }
        return true;
    }
    
    private GameObject SpawnObject(GameObject prefab, Vector3 position, ARPlane plane)
    {
        if (plane == null || prefab == null) return null;
        
        var obj = Instantiate(prefab, position, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        if (obj != null)
        {
            obj.transform.parent = plane.transform;
        }
        return obj;
    }
    
    private IEnumerator GrowObject(GameObject obj)
    {
        if (obj == null) yield break;
        
        var targetScale = Vector3.one * Random.Range(0.05f, 0.15f);
        obj.transform.localScale = Vector3.zero;
        
        float elapsed = 0;
        while (elapsed < growDuration && obj != null)
        {
            elapsed += Time.deltaTime;
            if (obj != null)
            {
                obj.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsed / growDuration);
            }
            yield return null;
        }
        
        if (obj != null) obj.transform.localScale = targetScale;
    }
    
    private void OnDestroy()
    {
        foreach (var objects in spawnedObjects.Values)
        {
            foreach (var obj in objects)
            {
                if (obj != null)
                    Destroy(obj);
            }
        }
        spawnedObjects.Clear();
        
        foreach (var puzzles in spawnedPuzzles.Values)
        {
            foreach (var puzzle in puzzles)
            {
                if (puzzle != null)
                    Destroy(puzzle);
            }
        }
        spawnedPuzzles.Clear();
    }
}