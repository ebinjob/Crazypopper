using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    public AudioClip poppersBackgroundMusic;
    public AudioClip applauseShort;
    public AudioClip awh;
    public AudioClip pop3;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float backgroundMusicVolume = 0.1f; 
    [Range(0f, 1f)]
    public float sfxVolume = 1f; 
    private AudioSource backgroundMusicSource;
    private AudioSource sfxSource;

    private void Awake()
    {       
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);         
        backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        backgroundMusicSource.clip = poppersBackgroundMusic;
        backgroundMusicSource.loop = true; 
        backgroundMusicSource.volume = backgroundMusicVolume; 
        backgroundMusicSource.Play();
    }

    public void PlayApplause()
    {
        sfxSource.PlayOneShot(applauseShort, sfxVolume); 
    }

    public void PlayAwh()
    {
        sfxSource.PlayOneShot(awh, sfxVolume); 
    }

    public void PlayPop()
    {
        sfxSource.PlayOneShot(pop3, sfxVolume); 
    }
}