using UnityEngine;
using UnityEngine.UI;

public class Destructible : MonoBehaviour
{
   public GameObject destroyedVersion;
   public Slider progressSlider;
   private Camera arCamera;

   void Start()
   {
       arCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
   }

   void Update()
   {
       if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
       {
           Ray ray = arCamera.ScreenPointToRay(Input.GetTouch(0).position);
           RaycastHit hit;

           if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
           {
               progressSlider.value += 0.1f; // Increases by 10%
               Instantiate(destroyedVersion, transform.position, transform.rotation);
               Destroy(gameObject);
           }
       }
   }
}