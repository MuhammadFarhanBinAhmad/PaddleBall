using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseOverLayUI : BaseOverLayInteraction
{
    [Header("OpenPauseOverlay")]
    [SerializeField] Button _openPauseOverlayButton;
    [SerializeField] GameObject _pauseOverlay;
    [Header("PauseOverlay")]
    [SerializeField] Button _openOptionsButton;
    [SerializeField] Button _resumeGameButton;
    [SerializeField] Button _restartGameButton;
    [SerializeField] Button _quitGameButton;
    [SerializeField] GameObject _pauseContent;
    [Header("OptionsOverlay")]
    [SerializeField] Button _returnToPauseMenu;
    [SerializeField] GameObject _optionsOverlay;
    [Header("Window And Resoultion")]
    [SerializeField] TMP_Dropdown _resolutionDropdown;
    [SerializeField] TMP_Dropdown _windowModeDropDown;
    readonly List<Resolution> _resolutions = new();
    int _currentResolutionIndex;
    [Header("Audio And Music")]
    [SerializeField] Slider _masterVolumeSlider;
    [SerializeField] Slider _sfxVolumeSlider;
    [SerializeField] Slider _musicVolumeSlider;
    [SerializeField] VOLUMETYPE _volumeType;





    [SerializeField]Animator _anim;
    private void Start()
    {
        _openPauseOverlayButton.onClick.AddListener(PlayOpenPauseOverlay);

        _resumeGameButton.onClick.AddListener(PlayClosePauseOverlay);

        _openOptionsButton.onClick.AddListener(PlayOpenOptionOverlay);

        _returnToPauseMenu.onClick.AddListener(PlayCloseOptionOverlay);


        SetupResolutions();
        SetupWindowModes();
        ApplySettings();
    }
    private void Update()
    {
        switch (_volumeType)
        {
            case VOLUMETYPE.MASTER:
                _masterVolumeSlider.value = AudioManager.Instance._masterVolume;
                break;
            case VOLUMETYPE.MUSIC:
                _musicVolumeSlider.value = AudioManager.Instance._musicVolume;
                break;
            case VOLUMETYPE.SFX:
                _sfxVolumeSlider.value = AudioManager.Instance._sfxvolume;
                break;
        }
    }

    public void OnSliderValueChanges()
    {
        switch (_volumeType)
        {
            case VOLUMETYPE.MASTER:
                AudioManager.Instance._masterVolume = _masterVolumeSlider.value;
                break;
            case VOLUMETYPE.MUSIC:
                AudioManager.Instance._musicVolume= _musicVolumeSlider.value;
                break;
            case VOLUMETYPE.SFX:
                AudioManager.Instance._sfxvolume = _sfxVolumeSlider.value;
                break;
        }
    }
    //PauseMenu
    public void PlayOpenPauseOverlay()
    {
         StartCoroutine(OpenPauseOverlay());
    }
    IEnumerator OpenPauseOverlay()
    {
        OpenOverlay(_pauseOverlay);
        _anim.SetTrigger("OpenPauseOverlay");
        AnimatorStateInfo state =
        _anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSecondsRealtime(state.length);
    }
    public void PlayClosePauseOverlay()
    {
        StartCoroutine(ClosePauseOverlay());
    }
    IEnumerator ClosePauseOverlay()
    {
        _anim.SetTrigger("ClosePauseOverlay");
        AnimatorStateInfo state =
        _anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSecondsRealtime(state.length+0.3f);
        CloseOverlay(_pauseOverlay);
    }
    //Option
    public void PlayOpenOptionOverlay()
    {
        StartCoroutine(OpenOptionOverlay());
    }
    IEnumerator OpenOptionOverlay()
    {
        _optionsOverlay.SetActive(true);
        OpenOverlay(_optionsOverlay);
        _anim.SetTrigger("ChangeToOptions");
        AnimatorStateInfo state =
        _anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSecondsRealtime(state.length);
        _pauseContent.SetActive(false);
    }
    public void PlayCloseOptionOverlay()
    {
        StartCoroutine(CloseOptionOverlay());
    }
    IEnumerator CloseOptionOverlay()
    {
        _pauseContent.SetActive(true);
        _anim.SetTrigger("ChangeToPause");
        AnimatorStateInfo state =
        _anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSecondsRealtime(state.length + 0.1f);
        _optionsOverlay.SetActive(false);
    }
    //Window and Resolution
    private void SetupResolutions()
    {
        _resolutionDropdown.ClearOptions();

        Resolution[] availableResolutions = Screen.resolutions;

        List<string> options = new();

        for (int i = 0; i < availableResolutions.Length; i++)
        {
            Resolution resolution = availableResolutions[i];

            string option = $"{resolution.width} x {resolution.height}";
            options.Add(option);

            _resolutions.Add(resolution);

            // Find the current resolution.
            if (resolution.width == Screen.currentResolution.width &&
                resolution.height == Screen.currentResolution.height)
            {
                _currentResolutionIndex = i;
            }
        }

        _resolutionDropdown.AddOptions(options);

        _resolutionDropdown.value = _currentResolutionIndex;
        _resolutionDropdown.RefreshShownValue();

        _resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    private void SetupWindowModes()
    {
        _windowModeDropDown.ClearOptions();

        List<string> modes = new()
        {
            "Windowless",
            "Window",
            "Fullscreen"
        };

        _windowModeDropDown.AddOptions(modes);

        // Set dropdown to current Unity window mode.
        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.FullScreenWindow:
                _windowModeDropDown.value = 0;
                break;

            case FullScreenMode.Windowed:
                _windowModeDropDown.value = 1;
                break;

            case FullScreenMode.ExclusiveFullScreen:
                _windowModeDropDown.value = 2;
                break;
        }

        _windowModeDropDown.RefreshShownValue();

        _windowModeDropDown.onValueChanged.AddListener(SetWindowMode);
    }

    private void SetResolution(int index)
    {
        if (index < 0 || index >= _resolutions.Count)
            return;

        _currentResolutionIndex = index;

        ApplySettings();
    }

    private void SetWindowMode(int index)
    {
        ApplySettings();
    }

    private void ApplySettings()
    {
        Resolution resolution = _resolutions[_currentResolutionIndex];

        FullScreenMode mode = _windowModeDropDown.value switch
        {
            0 => FullScreenMode.FullScreenWindow,     // Borderless
            1 => FullScreenMode.Windowed,             // Normal window
            2 => FullScreenMode.ExclusiveFullScreen,  // Exclusive fullscreen
            _ => FullScreenMode.Windowed
        };

        Screen.SetResolution(
            resolution.width,
            resolution.height,
            mode
        );

        // Resolution isn't meaningful for borderless fullscreen.
        _resolutionDropdown.interactable =
            mode != FullScreenMode.FullScreenWindow;
    }

}
