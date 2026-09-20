using UnityEngine;
using UnityEngine.UI;

public class PauseSettingsController : MonoBehaviour
{
    [Header("Settings UI")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pausePanel;

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string FullscreenKey = "Fullscreen";

    private void OnEnable()
    {
        LoadSettings();
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

        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ApplyFullscreen()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }

    private void LoadSettings()
    {
        masterVolumeSlider.value =
            PlayerPrefs.GetFloat(MasterVolumeKey, 1f);

        musicVolumeSlider.value =
            PlayerPrefs.GetFloat(MusicVolumeKey, 1f);

        sfxVolumeSlider.value =
            PlayerPrefs.GetFloat(SFXVolumeKey, 1f);

        fullscreenToggle.isOn =
            PlayerPrefs.GetInt(FullscreenKey, 1) == 1;

        ApplyFullscreen();
    }
}