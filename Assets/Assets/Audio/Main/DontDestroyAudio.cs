using UnityEngine;

public class DontDestroyAudio : MonoBehaviour 
{
    private static DontDestroyAudio instance = null;

    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
    }
}