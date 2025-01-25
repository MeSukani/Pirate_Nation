using UnityEngine;
using UnityEngine.UI;

public class Destructible : MonoBehaviour
{
   public GameObject destroyedVersion;
   public Slider progressSlider;
   public AudioSource destructionSound;
   private Camera arCamera;
   private GameEndManager endManager;

   void Start()
   {
       arCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
       endManager = FindObjectOfType<GameEndManager>();
   }

   void Update()
   {
       if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
       {
           Ray ray = arCamera.ScreenPointToRay(Input.GetTouch(0).position);
           RaycastHit hit;

           if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
           {
               progressSlider.value += 1f;
               if (destructionSound != null) destructionSound.Play();
               
               if (progressSlider.value >= progressSlider.maxValue)
               {
                   endManager.TriggerEndSequence();
               }
               
               Instantiate(destroyedVersion, transform.position, transform.rotation);
               Destroy(gameObject);
           }
       }
   }
}