using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioSource footstepSource;
    
    [Header("Player Sounds")]
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip jumpSound;
    public AudioClip landSound;
    
    [Header("Game Sounds")]
    public AudioClip collectSound;
    public AudioClip timerStartSound;
    public AudioClip timerEndSound;
    
    [Header("Background Music")]
    public AudioClip backgroundMusic;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float footstepVolume = 0.7f;
    
    [Header("Footstep Settings")]
    public float walkFootstepInterval = 0.5f;
    public float runFootstepInterval = 0.3f;
    
    private float footstepTimer = 0f;
    private bool isPlayingFootsteps = false;
    
    public static AudioManager Instance { get; private set; }
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        SetupAudioSources();
    }
    
    void Start()
    {
        PlayBackgroundMusic();
    }
    
    void SetupAudioSources()
    {
        // Create audio sources if they don't exist
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
        
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        
        if (footstepSource == null)
        {
            footstepSource = gameObject.AddComponent<AudioSource>();
            footstepSource.playOnAwake = false;
        }
        
        UpdateVolumes();
    }
    
    void Update()
    {
        UpdateVolumes();
    }
    
    void UpdateVolumes()
    {
        if (sfxSource != null) sfxSource.volume = sfxVolume;
        if (musicSource != null) musicSource.volume = musicVolume;
        if (footstepSource != null) footstepSource.volume = footstepVolume;
    }
    
    // Background Music
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }
    
    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
    
    // Footstep Sounds
    public void PlayFootsteps(bool isRunning, bool isGrounded)
    {
        if (!isGrounded)
        {
            StopFootsteps();
            return;
        }
        
        AudioClip footstepClip = isRunning ? runSound : walkSound;
        float interval = isRunning ? runFootstepInterval : walkFootstepInterval;
        
        if (footstepClip == null || footstepSource == null) return;
        
        footstepTimer += Time.deltaTime;
        
        if (footstepTimer >= interval)
        {
            footstepSource.PlayOneShot(footstepClip);
            footstepTimer = 0f;
            isPlayingFootsteps = true;
        }
    }
    
    public void StopFootsteps()
    {
        footstepTimer = 0f;
        isPlayingFootsteps = false;
    }
    
    // Jump Sound
    public void PlayJumpSound()
    {
        PlaySound(jumpSound);
    }
    
    // Land Sound
    public void PlayLandSound()
    {
        PlaySound(landSound);
    }
    
    // Collect Sound
    public void PlayCollectSound()
    {
        PlaySound(collectSound);
    }
    
    // Timer Sounds
    public void PlayTimerStartSound()
    {
        PlaySound(timerStartSound);
    }
    
    public void PlayTimerEndSound()
    {
        PlaySound(timerEndSound);
    }
    
    // Generic sound player
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    // Play sound at specific position
    public void PlaySoundAtPosition(AudioClip clip, Vector3 position)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, sfxVolume);
        }
    }
    
    // Volume controls
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
    }
    
    public void SetFootstepVolume(float volume)
    {
        footstepVolume = Mathf.Clamp01(volume);
    }
}

/*
SETUP INSTRUCTIONS:

1. Create an empty GameObject called "AudioManager"
2. Add this AudioManager script to it
3. Assign all your audio clips in the Inspector
4. The AudioManager will create AudioSources automatically if needed

Audio Sources created:
- sfxSource: For one-shot sounds (jump, land, collect, timer)
- musicSource: For background music (loops)
- footstepSource: For footstep sounds (walk/run)

The AudioManager uses Singleton pattern, so you can access it from anywhere:
AudioManager.Instance.PlayJumpSound();
*/