using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public GameObject menu;
    private OptionsMenu instance;

    private List<Resolution> resolutions;
    private void Awake()
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
    void Start()
    {
        menu.SetActive(false);
        // Manually define the desired resolutions
        resolutions = new List<Resolution>
        {
            new Resolution { width = 1080, height = 720 },
            new Resolution { width = 1600, height = 900 },
            new Resolution { width = 1920, height = 1080 }
        };

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

        // Set the volume slider to the current volume
        volumeSlider.value = AudioListener.volume;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void CloseOptions()
    {
        gameObject.SetActive(false);
    }
}

public struct Resolution
{
    public int width;
    public int height;
}

