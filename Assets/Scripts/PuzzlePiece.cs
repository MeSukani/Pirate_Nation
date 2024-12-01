using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    private Renderer pieceRenderer;
    private Color originalColor;
    private bool isHighlighted = false;
    
    private void Start()
    {
        // Get the renderer and save original color
        pieceRenderer = GetComponent<Renderer>();
        if (pieceRenderer?.material != null)
        {
            originalColor = pieceRenderer.material.color;
        }
        
        // Add required components if missing
        if (GetComponent<BoxCollider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }
    
    public void Toggle()
    {
        if (pieceRenderer?.material != null)
        {
            // Toggle highlight state
            isHighlighted = !isHighlighted;
            
            // Change color based on state
            pieceRenderer.material.color = isHighlighted ? Color.yellow : originalColor;
            
            // Simple scale animation
            transform.localScale = isHighlighted ? 
                transform.localScale * 1.2f : 
                transform.localScale / 1.2f;
        }
    }
}