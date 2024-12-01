using UnityEngine;

public class Piece : MonoBehaviour
{
    [HideInInspector]
    public int xIndex;
    [HideInInspector]
    public int yIndex;
    
    public int pieceType;
    public bool isPowerNode = false;

    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;    // Base sprite
    [SerializeField] private Sprite selectedSprite;  // Selected state
    [SerializeField] private Sprite nodeSprite;      // Power node sprite
    [SerializeField] private Sprite nodeSelectedSprite; // Selected power node sprite

    private Board board;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Set initial sprite based on whether it's a power node
        if (spriteRenderer)
        {
            spriteRenderer.sprite = isPowerNode ? nodeSprite : normalSprite;
        }
    }

    public void Init(int x, int y, Board boardRef, int type)
    {
        xIndex = x;
        yIndex = y;
        board = boardRef;
        pieceType = type;
    }

    private void OnMouseDown()
    {
        board.TryAddPieceToPath(this);
    }

    public void SelectPiece()
    {
        if (spriteRenderer)
        {
            // Use appropriate sprite based on whether it's a power node
            spriteRenderer.sprite = isPowerNode ? nodeSelectedSprite : selectedSprite;
        }
    }

    public void DeselectPiece()
    {
        if (spriteRenderer)
        {
            // Return to appropriate sprite based on whether it's a power node
            spriteRenderer.sprite = isPowerNode ? nodeSprite : normalSprite;
        }
    }

    public void SetType(int newType)
    {
        pieceType = newType;
    }
}