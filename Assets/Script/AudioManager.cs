using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;   
    public AudioSource sfxSource;    
    
    [Header("BGM")]
    public AudioClip bgmStartScene;   
    public AudioClip bgmGameplay;     
    public AudioClip bgmGameOver;     
    
    [Header("SFX")]
    public AudioClip sfxJump;        
    public AudioClip sfxHit;          
    public AudioClip sfxDie;          
    public AudioClip sfxCoin;        
    public AudioClip sfxHeal;         
    public AudioClip sfxSpeedBoost;   
    public AudioClip sfxButtonClick;  

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ApplySettings();
    }
    
    public void ApplySettings()
    {
        if (musicSource != null)
            musicSource.mute = !GameData.IsMusicOn;
        if (sfxSource != null)
            sfxSource.mute = !GameData.IsSfxOn;
    }
    
    public void PlayBGM(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopBGM()
    {
        if (musicSource != null) musicSource.Stop();
    }

    public void PlayStartBGM()    => PlayBGM(bgmStartScene);
    public void PlayGameplayBGM() => PlayBGM(bgmGameplay);
    public void PlayGameOverBGM() => PlayBGM(bgmGameOver);
    
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayJump()        => PlaySFX(sfxJump);
    public void PlayHit()         => PlaySFX(sfxHit);
    public void PlayDie()         => PlaySFX(sfxDie);
    public void PlayCoin()        => PlaySFX(sfxCoin);
    public void PlayHeal()        => PlaySFX(sfxHeal);
    public void PlaySpeedBoost()  => PlaySFX(sfxSpeedBoost);
    public void PlayButtonClick() => PlaySFX(sfxButtonClick);
    
    public void SetMusic(bool isOn)
    {
        GameData.IsMusicOn = isOn;
        if (musicSource != null) musicSource.mute = !isOn;
    }

    public void SetSFX(bool isOn)
    {
        GameData.IsSfxOn = isOn;
        if (sfxSource != null) sfxSource.mute = !isOn;
    }
}