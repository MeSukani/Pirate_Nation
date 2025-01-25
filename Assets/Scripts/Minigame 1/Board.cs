using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Board : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource pieceClickSound;
    public AudioSource pathCompleteSound;

    [Header("Board Settings")]
    public int width;
    public int height;
    public float spacing = 0.7f;

    [Header("Game Settings")]
    public int minimumPathLength = 3;
    public int pointsPerPiece = 10;
    public int numberOfPowerNodes = 2;

    [Header("Prefabs")]
    [Tooltip("Add base pieces (0-4) then node versions (5-9) in same order")]
    public GameObject[] pieces;   // First 5 are base pieces, next 5 are node versions

    private GameObject[,] allPieces;
    private List<Piece> currentPath = new List<Piece>();
    private bool isProcessingMatch = false;
    private List<Vector2Int> powerNodePositions = new List<Vector2Int>();

    public GameUIManager uiManager;

    void Start()
    {
        allPieces = new GameObject[width, height];
        SetUp();
    }

    private void SetUp()
    {
        float offsetX = -width / 2f + 0.5f;
        float offsetY = -height / 2f + 0.5f;

        // Create regular pieces
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CreateRegularPiece(x, y, offsetX, offsetY);
            }
        }

        // Create and validate power nodes
        CreateInitialPowerNodes();
        ValidateAllNodes();
    }

    private void CreateRegularPiece(int x, int y, float offsetX, float offsetY)
    {
        Vector2 tempPosition = new Vector2(
            (x + offsetX) * spacing,
            (y + offsetY) * spacing
        );

        int pieceIndex = Random.Range(0, 5);
        GameObject piece = Instantiate(pieces[pieceIndex], tempPosition, Quaternion.identity);
        piece.transform.parent = transform;
        piece.name = $"Piece({x},{y})";

        Piece pieceComponent = piece.GetComponent<Piece>();
        if (pieceComponent == null)
        {
            pieceComponent = piece.AddComponent<Piece>();
        }
        pieceComponent.Init(x, y, this, pieceIndex);

        allPieces[x, y] = piece;
    }

    private void CreateInitialPowerNodes()
    {
        powerNodePositions.Clear();
        
        for (int i = 0; i < numberOfPowerNodes; i++)
        {
            // Keep trying until we place each node
            bool nodePlaced = false;
            int attempts = 0;
            const int maxAttempts = 50;

            while (!nodePlaced && attempts < maxAttempts)
            {
                int x = Random.Range(0, width);
                int y = Random.Range(0, height);
                Vector2Int pos = new Vector2Int(x, y);

                if (!powerNodePositions.Contains(pos))
                {
                    int pieceType = FindValidPieceType(x, y);
                    CreatePowerNode(x, y, pieceType);
                    powerNodePositions.Add(pos);
                    nodePlaced = true;
                }

                attempts++;
            }
        }
    }

    private void CreatePowerNode(int x, int y, int pieceType)
    {
        Vector3 position = allPieces[x, y].transform.position;
        Destroy(allPieces[x, y]);

        int nodeIndex = pieceType + 5;
        GameObject nodePiece = Instantiate(pieces[nodeIndex], position, Quaternion.identity);
        nodePiece.transform.parent = transform;
        nodePiece.name = $"NodePiece({x},{y})";

        Piece nodeComponent = nodePiece.GetComponent<Piece>();
        if (nodeComponent != null)
        {
            nodeComponent.Init(x, y, this, pieceType);
            nodeComponent.isPowerNode = true;
        }

        allPieces[x, y] = nodePiece;
        EnsureSufficientMatches(x, y, pieceType);
    }

    private void EnsureSufficientMatches(int nodeX, int nodeY, int targetType)
    {
        List<Vector2Int> adjacentPositions = GetAdjacentPositions(nodeX, nodeY);
        int matchingCount = 0;

        // Count current matches
        foreach (Vector2Int pos in adjacentPositions)
        {
            Piece piece = allPieces[pos.x, pos.y].GetComponent<Piece>();
            if (piece != null && !piece.isPowerNode && piece.pieceType == targetType)
            {
                matchingCount++;
            }
        }

        // If we need more matches, force convert some adjacent pieces
        if (matchingCount < 2)
        {
            // Shuffle adjacent positions
            adjacentPositions = adjacentPositions.OrderBy(x => Random.value).ToList();
            
            // Convert pieces until we have at least 2 matches
            int piecesToConvert = 2 - matchingCount;
            int converted = 0;

            foreach (Vector2Int pos in adjacentPositions)
            {
                if (converted >= piecesToConvert) break;

                Piece piece = allPieces[pos.x, pos.y].GetComponent<Piece>();
                if (piece != null && !piece.isPowerNode && piece.pieceType != targetType)
                {
                    ForceConvertPiece(pos.x, pos.y, targetType);
                    converted++;
                }
            }
        }
    }

    private List<Vector2Int> GetAdjacentPositions(int x, int y)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                
                int newX = x + i;
                int newY = y + j;
                
                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    positions.Add(new Vector2Int(newX, newY));
                }
            }
        }
        
        return positions;
    }

    private void ForceConvertPiece(int x, int y, int targetType)
    {
        Vector3 position = allPieces[x, y].transform.position;
        Destroy(allPieces[x, y]);

        GameObject newPiece = Instantiate(pieces[targetType], position, Quaternion.identity);
        newPiece.transform.parent = transform;
        newPiece.name = $"Piece({x},{y})";

        Piece newPieceComponent = newPiece.GetComponent<Piece>();
        if (newPieceComponent != null)
        {
            newPieceComponent.Init(x, y, this, targetType);
        }

        allPieces[x, y] = newPiece;
    }

    public void ValidateAllNodes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Piece piece = allPieces[x, y]?.GetComponent<Piece>();
                if (piece != null && piece.isPowerNode)
                {
                    EnsureSufficientMatches(x, y, piece.pieceType);
                }
            }
        }
    }

    private int FindValidPieceType(int x, int y)
    {
        Dictionary<int, int> typeMatches = new Dictionary<int, int>();
        
        // Count matches for each type
        for (int type = 0; type < 5; type++)
        {
            int matches = GetMatchingAdjacentCount(x, y, type);
            typeMatches[type] = matches;
        }
        
        // First try to find types with at least 2 matches
        var validTypes = typeMatches.Where(kv => kv.Value >= 2).ToList();
        if (validTypes.Any())
        {
            return validTypes[Random.Range(0, validTypes.Count)].Key;
        }
        
        // If no type has 2+ matches, pick random type (we'll force matches later)
        return Random.Range(0, 5);
    }

    private int GetMatchingAdjacentCount(int x, int y, int pieceType)
    {
        int matches = 0;
        
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                
                int newX = x + i;
                int newY = y + j;
                
                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    Piece piece = allPieces[newX, newY].GetComponent<Piece>();
                    if (piece != null && !piece.isPowerNode && piece.pieceType == pieceType)
                    {
                        matches++;
                    }
                }
            }
        }
        return matches;
    }

    public void TryAddPieceToPath(Piece piece)
    {
        if (uiManager != null && !uiManager.HasMovesLeft())
        {
            return; // Don't allow more moves if game is over
        }

        if (isProcessingMatch) return;
         if (pieceClickSound != null) pieceClickSound.Play();

        if (currentPath.Count == 0)
        {
            currentPath.Add(piece);
            piece.SelectPiece();
            return;
        }

        Piece lastPiece = currentPath[currentPath.Count - 1];
        bool isAdjacent = IsAdjacent(lastPiece, piece);
        bool isSameType = lastPiece.pieceType == piece.pieceType;
        bool isNotInPath = !currentPath.Contains(piece);

        if (piece.isPowerNode)
        {
            if (isAdjacent && isSameType)
            {
                currentPath.Add(piece);
                piece.SelectPiece();
                
                if (currentPath.Count >= minimumPathLength)
                {
                    ProcessPath();
                }
                else
                {
                    ClearPath();
                }
            }
            else
            {
                ClearPath();
                currentPath.Add(piece);
                piece.SelectPiece();
            }
            return;
        }

        if (isAdjacent && isSameType && isNotInPath)
        {
            currentPath.Add(piece);
            piece.SelectPiece();
        }
        else
        {
            ClearPath();
            currentPath.Add(piece);
            piece.SelectPiece();
        }
    }

    private void ProcessPath()
    {
        if (currentPath.Count >= minimumPathLength && HasPowerNode())
        {
            isProcessingMatch = true;

            int score = currentPath.Count * pointsPerPiece;
            if (uiManager != null)
            {
                uiManager.AddScore(score);
                uiManager.DecrementMoves();
            }
            if (pathCompleteSound != null) pathCompleteSound.Play();

            // Only process the board if the game isn't over
            if (uiManager != null && uiManager.HasMovesLeft())
            {
                // First replace non-node pieces
                foreach (Piece piece in currentPath)
                {
                    if (!piece.isPowerNode)
                    {
                        ReplacePiece(piece.xIndex, piece.yIndex, false);
                    }
                }

                // Then handle nodes
                foreach (Piece piece in currentPath)
                {
                    if (piece.isPowerNode)
                    {
                        int newType = FindValidPieceType(piece.xIndex, piece.yIndex);
                        powerNodePositions.Remove(new Vector2Int(piece.xIndex, piece.yIndex));
                        CreatePowerNode(piece.xIndex, piece.yIndex, newType);
                        powerNodePositions.Add(new Vector2Int(piece.xIndex, piece.yIndex));
                    }
                }

                ValidateAllNodes();
            }

            currentPath.Clear();
            isProcessingMatch = false;
        }
    }


    private void ReplacePiece(int x, int y, bool isNode)
    {
        Vector3 position = allPieces[x, y].transform.position;
        Destroy(allPieces[x, y]);

        int newType = isNode ? FindValidPieceType(x, y) : Random.Range(0, 5);
        int prefabIndex = isNode ? newType + 5 : newType;

        GameObject newPiece = Instantiate(pieces[prefabIndex], position, Quaternion.identity);
        newPiece.transform.parent = transform;
        newPiece.name = isNode ? $"NodePiece({x},{y})" : $"Piece({x},{y})";

        Piece newPieceComponent = newPiece.GetComponent<Piece>();
        if (newPieceComponent != null)
        {
            newPieceComponent.Init(x, y, this, newType);
            newPieceComponent.isPowerNode = isNode;
        }

        allPieces[x, y] = newPiece;
    }

    public bool IsAdjacent(Piece p1, Piece p2)
    {
        return Mathf.Abs(p1.xIndex - p2.xIndex) <= 1 && 
               Mathf.Abs(p1.yIndex - p2.yIndex) <= 1;
    }

    public bool HasPowerNode()
    {
        return currentPath.Exists(piece => piece.isPowerNode);
    }

    public void ClearPath()
    {
        foreach (Piece piece in currentPath)
        {
            if (piece != null)
            {
                piece.DeselectPiece();
            }
        }
        currentPath.Clear();
    }
}