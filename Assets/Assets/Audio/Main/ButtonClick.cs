using UnityEngine;

public class ButtonSound : MonoBehaviour
{
   public AudioSource buttonSound;

   public void PlayButtonSound()
   {
       if (buttonSound != null)
       {
           buttonSound.Play();
       }
   }
  
}