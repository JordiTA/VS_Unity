using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.ComponentModel;
using Unity.VisualScripting;
using System;

public class MenuController : MonoBehaviour{

    public static MenuController _MENUCONTROLLER; //SINGLETON

    [Header("Volume Settings")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSliderValue = null;
    [SerializeField] private float defaultVolume = 0.5f;

    [Header("Graphics Settings")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;
    [SerializeField] private float defaultBrightness = 0.5f;

    [Space(10)]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullScreenToggle;

    private int qualityLevel;
    private bool isFullScreen;
    private float brightnessLevel;

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;

    [Header("Levels To Load")]
    [SerializeField] private string newGameLevel;

    [Header("Resolution Dropdowns")]
    [SerializeField] private TMP_Dropdown resolutionDropdown = null;
    [Space(10)]
    [Header("Player Name Field")]
    [SerializeField]
    private TMP_Text playerNameDialog;


    private Resolution[] resolutions;
    private int actualHighScore;
    private string playerName;

    private void Awake(){
        if (_MENUCONTROLLER != null && _MENUCONTROLLER != this)
        {
            Destroy(this.gameObject);
        }else
        {
            _MENUCONTROLLER = this;
            DontDestroyOnLoad(this);
        }
    }
    private void Start() {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height){ currentResolutionIndex = i; }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    
    public void SetResolution(int _resolutionIndex)
    {
        Resolution resolution = resolutions[_resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void CreditsScene(){
        SceneManager.LoadScene("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetVolume(float _volume)
    {
        AudioListener.volume = _volume;
        volumeTextValue.text = _volume.ToString("0.0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    public void SetBrightness(float _brightness)
    {
        brightnessLevel = _brightness;
        brightnessTextValue.text = _brightness.ToString("0.0");
    }

    public void SetFullScreen(bool _isFullScreen)
    {
        isFullScreen = _isFullScreen;
    }

    public void SetQuality(int _qualityIndex)
    {
        qualityLevel = _qualityIndex;
    }

    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", brightnessLevel);

        PlayerPrefs.SetInt("masterQuality", qualityLevel);
        QualitySettings.SetQualityLevel(qualityLevel);

        PlayerPrefs.SetInt("masterFullscreen", isFullScreen ? 1 : 0);
        Screen.fullScreen = isFullScreen;

        StartCoroutine(ConfirmationBox());
    }

    public void ResetButton(string _MenuType)
    {
        if (_MenuType == "Graphics")
        {
            brightnessSlider.value = defaultBrightness;
            brightnessTextValue.text = defaultBrightness.ToString("0.0");

            qualityDropdown.value = 1;

            QualitySettings.SetQualityLevel(1);

            fullScreenToggle.isOn = true;
            Screen.fullScreen = true;

            Resolution currentResoltion = Screen.currentResolution;
            Screen.SetResolution(currentResoltion.width, currentResoltion.height, Screen.fullScreen);
            resolutionDropdown.value = resolutions.Length;
            GraphicsApply();
        }
        if (_MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSliderValue.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }
    }

    public void MainMenu(){
        SceneManager.LoadScene("MainMenu");
    }
    public void RaceScene(){
        SceneManager.LoadScene("Race");
    }
    public void LostScene(){
        SceneManager.LoadScene("Lost");
    }
    public void SaveHighScore(int _highScore)
    {
        actualHighScore = _highScore;
        SceneManager.LoadScene("Highscore");
    }
    public int GetHighScore()
    {
        return actualHighScore;
    }
    public void SavePlayerName(){
        playerName = playerNameDialog.text;
        SceneManager.LoadScene(newGameLevel);
    }
    public void PracticeScene(){
        SceneManager.LoadScene("Practice");
    }
    public string GetPlayerName(){
        return playerName;
    }

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }
}
