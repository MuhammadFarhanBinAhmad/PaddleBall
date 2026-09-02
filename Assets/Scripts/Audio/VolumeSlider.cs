using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] Slider _volumeSlider;
    [SerializeField] VOLUMETYPE _volumeType;

    private void Update()
    {
        switch (_volumeType)
        {
            case VOLUMETYPE.MASTER:
                _volumeSlider.value = AudioManager.Instance._masterVolume;
                break;
            case VOLUMETYPE.MUSIC:
                _volumeSlider.value = AudioManager.Instance._musicVolume;
                break;
            case VOLUMETYPE.SFX:
                _volumeSlider.value = AudioManager.Instance._sfxvolume;
                break;
        }
    }

    public void OnSliderValueChanges()
    {
        switch (_volumeType)
        {
            case VOLUMETYPE.MASTER:
                AudioManager.Instance._masterVolume = _volumeSlider.value;
                break;
            case VOLUMETYPE.MUSIC:
                AudioManager.Instance._musicVolume = _volumeSlider.value;
                break;
            case VOLUMETYPE.SFX:
                AudioManager.Instance._sfxvolume = _volumeSlider.value;
                break;
        }
    }
}
