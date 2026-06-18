using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup Mixer;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Slider EffectSlider, MusicSlider;
    [SerializeField] private Toggle fullScreen;
    private Resolution[] resolutions;

    private void Awake()
    {
        ChangeMusicVolume(PlayerPrefs.GetFloat("MusicPreference", 0.5f));
        ChangeEffectsVolume(PlayerPrefs.GetFloat("EffectsPreference", 0.5f));
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            List<string> options = new();
            resolutions = Screen.resolutions;
            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRateRatio + "Hz";
                options.Add(option);
                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                    currentResolutionIndex = i;
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.RefreshShownValue();
            LoadSettings(currentResolutionIndex);
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void OnDisable()
    {
        PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetFloat("MusicPreference", MusicSlider.value);
        PlayerPrefs.SetFloat("EffectsPreference", EffectSlider.value);
    }

    public void ChangeMusicVolume(float Musicvolume)
    {
        Mixer.audioMixer.SetFloat("MusicVolume", NumToDecibel(Musicvolume));
    }

    public void ChangeEffectsVolume(float Effectsvolume)
    {
        Mixer.audioMixer.SetFloat("EffectsVolume",NumToDecibel(Effectsvolume));
    }

    public void LoadSettings(int currentResolutionIndex)
    {
        MusicSlider.value = PlayerPrefs.GetFloat("MusicPreference");
        EffectSlider.value = PlayerPrefs.GetFloat("EffectsPreference");
        
        fullScreen.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));

        if (PlayerPrefs.HasKey("ResolutionPreference"))
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutionDropdown.value = currentResolutionIndex;

        if (PlayerPrefs.HasKey("FullscreenPreference"))
            Screen.fullScreen = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        else
            Screen.fullScreen = true;
    }

    public float NumToDecibel(float num) => Mathf.Log10(num) * 20;
}