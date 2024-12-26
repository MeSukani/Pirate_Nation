using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PortalViewController : MonoBehaviour
{
    public Camera arCamera;              // Main AR Camera
    public GameObject portalFrame;       // The door frame object
    public LayerMask portalWorldLayer;   // Layer containing terrain and portal world objects
    public Material stencilMask;         // Material for the portal mask

    private Camera portalCamera;         // Camera that renders only portal world
    private RenderTexture renderTexture;

    void Start()
    {
        // Create portal camera
        GameObject portalCamObj = new GameObject("Portal Camera");
        portalCamera = portalCamObj.AddComponent<Camera>();
        
        // Setup portal camera
        portalCamera.clearFlags = CameraClearFlags.SolidColor;
        portalCamera.backgroundColor = Color.clear;
        portalCamera.cullingMask = portalWorldLayer; // Only render portal world layer
        
        // Create render texture
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        portalCamera.targetTexture = renderTexture;
        
        // Assign render texture to portal material
        stencilMask.mainTexture = renderTexture;
    }

    void LateUpdate()
    {
        // Make portal camera follow AR camera
        portalCamera.transform.position = arCamera.transform.position;
        portalCamera.transform.rotation = arCamera.transform.rotation;
        
        // Update portal camera properties to match AR camera
        portalCamera.fieldOfView = arCamera.fieldOfView;
        portalCamera.nearClipPlane = arCamera.nearClipPlane;
        portalCamera.farClipPlane = arCamera.farClipPlane;
    }
}