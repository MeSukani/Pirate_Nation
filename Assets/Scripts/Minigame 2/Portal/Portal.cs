using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Portal : MonoBehaviour
{

    public Material[] materials;
    public Transform device;
    private GameObject dimension;  // Reference for your environment
    private GameObject quad;
    bool wasInfront;
    bool inOtherWorld;
    bool hasCollided;

    public AudioSource portalSound;
    void Start()
    {
        if (device == null)
        {
            device = Camera.main.transform;
        }
        SetMaterials(false);
        
        quad = GameObject.Find("XR Origin/Camera Offset/Main Camera/Quad");
        if (portalSound == null)
        {
            GameObject soundObj = new GameObject("PortalSound");
            soundObj.transform.parent = transform;
            portalSound = soundObj.AddComponent<AudioSource>();
            portalSound.clip = Resources.Load<AudioClip>("PortalSound");           
            portalSound.playOnAwake = false;
            portalSound.spatialBlend = 1f;
        }
        
        // Enable them when portal spawns
        if (SceneObjectManager.Instance != null)
        {
            SceneObjectManager.Instance.dimension.SetActive(true);
            SceneObjectManager.Instance.quad.SetActive(true);
        }

        

    }

    void SetMaterials(bool fullRender)
    {
        var stencilTest = fullRender ? CompareFunction.NotEqual : CompareFunction.Equal;
            foreach (var mat in materials)
            {
                mat.SetInt("_StencilTest", (int)stencilTest);
            }
    }

    bool GetIsInFront()
    {
        Vector3 worldPos = device.position + device.forward * Camera.main.nearClipPlane;
        Vector3 pos = transform.InverseTransformPoint(worldPos);
        return pos.z >=0 ? true: false;
    }
    void OnTriggerEnter(Collider other) 
    {
        if (other.transform != device)
        
            return;
            wasInfront = GetIsInFront();
            hasCollided = true;
            portalSound.Play();

        
    }

    void OnTriggerExit(Collider other) 
    {
        if (other.transform != device)
        
            return;
            hasCollided = false;
            portalSound.Play();
    }

    void whileCameraColliding()
    {
        if(!hasCollided)
        return;

        bool isInFront = GetIsInFront();
        if ((isInFront && !wasInfront) || (wasInfront && !isInFront))
        {
            inOtherWorld = ! inOtherWorld;
            SetMaterials(inOtherWorld);
        }
        wasInfront = isInFront;
    }

    void OnDestroy() 
    {
        SetMaterials(true);
    }

    // Update is called once per frame
    void Update()
    {
        whileCameraColliding();
    }
}
