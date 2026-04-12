using UnityEngine;

// ติดไว้ที่ Empty GameObject ชื่อ "AudioManager" ใน StartScene
// ใช้ DontDestroyOnLoad เพื่อให้เสียงไม่ขาดตอนเปลี่ยน Scene
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;   // สำหรับเพลงพื้นหลัง (loop)
    public AudioSource sfxSource;     // สำหรับเสียง Effect
    
    [Header("BGM")]
    public AudioClip bgmStartScene;   // เพลงหน้า Start
    public AudioClip bgmGameplay;     // เพลงหน้า Gameplay
    public AudioClip bgmGameOver;     // เพลงหน้า Game Over
    
    [Header("SFX")]
    public AudioClip sfxJump;         // เสียงกระโดด
    public AudioClip sfxHit;          // เสียงโดน Damage
    public AudioClip sfxDie;          // เสียงตาย
    public AudioClip sfxCoin;         // เสียงเก็บ Coin
    public AudioClip sfxHeal;         // เสียงเก็บ Heal
    public AudioClip sfxSpeedBoost;   // เสียงเก็บ SpeedBoost
    public AudioClip sfxButtonClick;  // เสียงกดปุ่ม

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

    // ================================================================
    //  Apply Settings จาก GameData
    // ================================================================
    public void ApplySettings()
    {
        if (musicSource != null)
            musicSource.mute = !GameData.IsMusicOn;
        if (sfxSource != null)
            sfxSource.mute = !GameData.IsSfxOn;
    }

    // ================================================================
    //  BGM
    // ================================================================
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

    // ================================================================
    //  SFX
    // ================================================================
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

    // ================================================================
    //  Toggle Music / SFX (เรียกจาก Toggle ใน Option Panel)
    // ================================================================
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