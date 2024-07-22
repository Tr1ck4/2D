using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public GameObject menu;
    private List<Resolution> resolutions;
    private AudioSource[] audioSources;
    private bool isMenuActive = false;

    void Start()
    {
        menu.SetActive(isMenuActive);

        // Define the available resolutions
        resolutions = new List<Resolution>
        {
            new Resolution { width = 1080, height = 720 },
            new Resolution { width = 1600, height = 900 },
            new Resolution { width = 1920, height = 1080 }
        };

        // Initialize resolution dropdown
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Count; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution); // Add listener for dropdown changes

        // Initialize volume slider
        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(SetVolume);

        audioSources = FindObjectsOfType<AudioSource>();

        // Load settings from file
        LoadSettings();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.Windowed); // Ensure it's windowed
        SaveSettings(); // Save settings when resolution is changed
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.volume = volume;
        }
        SaveSettings(); // Save settings when volume is changed
    }

    public void ToggleOptionsMenu()
    {
        isMenuActive = !isMenuActive;
        menu.SetActive(isMenuActive);
    }

    private void SaveSettings()
    {
        SettingsData settings = new SettingsData
        {
            resolutionIndex = resolutionDropdown.value,
            volume = volumeSlider.value
        };

        string json = JsonUtility.ToJson(settings);
        File.WriteAllText(GetSettingsFilePath(), json);
    }

    private void LoadSettings()
    {
        string path = GetSettingsFilePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SettingsData settings = JsonUtility.FromJson<SettingsData>(json);

            resolutionDropdown.value = settings.resolutionIndex;
            volumeSlider.value = settings.volume;

            // Apply settings
            SetResolution(settings.resolutionIndex);
            SetVolume(settings.volume);
        }
    }

    private string GetSettingsFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "settingsData.json");
    }
}

[System.Serializable]
public class SettingsData
{
    public int resolutionIndex;
    public float volume;
}

public struct Resolution
{
    public int width;
    public int height;
}
