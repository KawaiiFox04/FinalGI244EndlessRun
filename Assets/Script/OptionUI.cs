using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    public Toggle bgmToggle;
    public Toggle sfxToggle;

    void OnEnable()
    {
        bgmToggle.isOn = PlayerPrefs.GetInt("BGM", 1) == 1;
        sfxToggle.isOn = PlayerPrefs.GetInt("SFX", 1) == 1;
    }

    public void OnBGMToggle(bool isOn)
    {
        PlayerPrefs.SetInt("BGM", isOn ? 1 : 0);
        PlayerPrefs.Save();
        // AudioManager.instance.SetBGM(isOn);
    }

    public void OnSFXToggle(bool isOn)
    {
        PlayerPrefs.SetInt("SFX", isOn ? 1 : 0);
        PlayerPrefs.Save();
        // AudioManager.instance.SetSFX(isOn);
    }
}