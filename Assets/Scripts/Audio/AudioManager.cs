using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public enum MUSIC_TRANSISTION
{
    DAY = 0,
    NIGHT = 1,
}
public enum VOLUMETYPE
{
    MASTER,
    MUSIC,
    SFX
}
public class AudioManager : MonoBehaviour
{


    [Header("Volume")]
    [Range(0,1)]
    public float _masterVolume=1;
    [Range(0, 1)]
    public float _musicVolume =1;
    [Range(0, 1)]
    public float _sfxvolume = 1;

    Bus _masterBus;
    Bus _musicBus;
    Bus _sfxBus;

    List<EventInstance> _eventInstance = new List<EventInstance>();
    List<StudioEventEmitter> _studioEventEmitter = new List<StudioEventEmitter>();

    EventInstance _musicEventInstance;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            print("more than one audio manager exist in scene");

        Instance = this;

        _masterBus = RuntimeManager.GetBus("bus:/");
        _musicBus = RuntimeManager.GetBus("bus:/Music");
        _sfxBus = RuntimeManager.GetBus("bus:/SFX");

    }
    private void Start()
    {
        InitializeMusic(FmodEvent.Instance.music_PlayScenes);
    }
    private void Update()
    {
        _masterBus.setVolume( _masterVolume );
        _musicBus.setVolume( _musicVolume );
        _sfxBus.setVolume(_sfxvolume );
    }
    public void PlayOneShot(EventReference sound, Vector3 worldpos)
    {
        RuntimeManager.PlayOneShot(sound, worldpos);
    }
    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        _eventInstance.Add(eventInstance);
        return eventInstance;
    }


    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameobject)
    {
        StudioEventEmitter emitter = emitterGameobject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        _studioEventEmitter.Add(emitter);
        return emitter;

    }

    void InitializeMusic(EventReference musicEventRef)
    {
        _musicEventInstance = CreateEventInstance(musicEventRef);
        _musicEventInstance.start();
    }
    void CleanUp()
    {
        //stop all created instance
        foreach (EventInstance eventInstance in _eventInstance)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        //stop all created instance
        foreach (StudioEventEmitter emitter in _studioEventEmitter)
        {
            emitter.Stop();
        }
    }
    public void SetMusicArea(MUSIC_TRANSISTION area)
    {
        _musicEventInstance.setParameterByName("MusicChange", (float)area);
    }
    private void OnDestroy()
    {
        CleanUp();
    }
}
