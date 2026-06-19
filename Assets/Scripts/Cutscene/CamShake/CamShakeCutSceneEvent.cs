using UnityEngine;

public class CamShakeCutSceneEvent : BaseCutsceneEvent
{

    [SerializeField] CameraShake _cameraShake;

    float _duration;
    float _trauma;


    public override void EndEvent()
    {
        NotifyFinished();
    }

    public override void ExecuteEvent()
    {
        _cameraShake.StartShake(_duration, _trauma);
        EndEvent();
    }
    public override void SetUpContent(SO_CutSceneEventContent content)
    {
        base.SetUpContent(content);
        _duration = content._duration;
        _trauma = content._trauma;
    }

}
