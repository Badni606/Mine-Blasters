using UnityEngine;

public class AudioManager : MonoBehaviour
{ 
    public static AudioManager instance;
    AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void PlaySFX(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public AudioClip ButtonClickSfx;
    public void PlayButtonSfx()
    {
        audioSource.PlayOneShot(ButtonClickSfx);
    }

    

}
