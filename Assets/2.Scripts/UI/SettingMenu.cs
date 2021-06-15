using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class SettingMenu : MonoBehaviour
{
    public static SettingMenu instance;

    public AudioMixer audioMixer;
    private TMP_Dropdown resolutionDropdown;
    private TMP_Dropdown graphicsDropdown;
    private Slider volumeSlider;
    private Button exitBtn;

    private Resolution[] resolutions;


    public void Test()
    {
        //1. AudioListener 사용
        AudioListener.volume = 1.0f;
        //2. AudioMixer 사용
/*        AudioSource audio = GetComponent<AudioSource>();
        audio.outputAudioMixerGroup = Resources.Load<AudioMixerGroup>("");*/
    }


    private void Awake()
    {
        // Only 1 Game Manager can exist at a time
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

    private void Start()
    {
        resolutionDropdown = transform.Find("Background/Resolution/Dropdown").GetComponent<TMP_Dropdown>();
        if (resolutionDropdown != null)
        {
            InitResolution();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }

        graphicsDropdown = transform.Find("Background/Graphics/Dropdown").GetComponent<TMP_Dropdown>();
        if(graphicsDropdown != null)
        {
            graphicsDropdown.onValueChanged.AddListener(SetGraphicQuality);
        }

        volumeSlider = GetComponentInChildren<Slider>(true);
        if(volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        exitBtn = GetComponentInChildren<Button>(true);
        if (exitBtn != null)
        {
            exitBtn.onClick.AddListener(CloseSettingMenu);
        }

        instance.gameObject.SetActive(false);
    }

    private void CloseSettingMenu()
    {
        if(SceneManager.GetActiveScene().name == "MenuScene")
        {
            NPC_DruidMia mia = FindObjectOfType<NPC_DruidMia>();
            if (mia != null)
                mia.EndTalkVoice();
        }

        //Time.timeScale = 1f;
        instance.gameObject.SetActive(false);
        UISound.ClickButton();
    }

    private void InitResolution()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
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

    }

    private void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }


    public void SetVolume(float value)
    {
        //audioMixer.SetFloat("Volume", value);
        AudioListener.volume = value;
        print("Volume : " + value);
    }

    public void SetGraphicQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool state)
    {
        Screen.fullScreen = state;
    }
}
