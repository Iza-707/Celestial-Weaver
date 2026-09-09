using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("Settings UI")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string FullscreenKey = "Fullscreen";

    private void Start()
    {
        bool fullscreen = PlayerPrefs.GetInt(
            FullscreenKey,
            1
        ) == 1;

        fullscreenToggle.isOn = fullscreen;

        ApplyFullscreen();
    }

    public void OpenSettings()
    {
        LoadSettings();
        settingsPanel.SetActive(true);
    }

    public void SaveAndExit()
    {
        SaveSettings();
        settingsPanel.SetActive(false);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(
            MasterVolumeKey,
            masterVolumeSlider.value
        );

        PlayerPrefs.SetFloat(
            MusicVolumeKey,
            musicVolumeSlider.value
        );

        PlayerPrefs.SetFloat(
            SFXVolumeKey,
            sfxVolumeSlider.value
        );

        PlayerPrefs.SetInt(
            FullscreenKey,
            fullscreenToggle.isOn ? 1 : 0
        );

        PlayerPrefs.Save();

        ApplyFullscreen();
    }

    public void LoadSettings()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat(
            MasterVolumeKey,
            1f
        );

        musicVolumeSlider.value = PlayerPrefs.GetFloat(
            MusicVolumeKey,
            1f
        );

        sfxVolumeSlider.value = PlayerPrefs.GetFloat(
            SFXVolumeKey,
            1f
        );

        fullscreenToggle.isOn = PlayerPrefs.GetInt(
            FullscreenKey,
            1
        ) == 1;

        ApplyFullscreen();
    }

    public void ApplyFullscreen()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }
}