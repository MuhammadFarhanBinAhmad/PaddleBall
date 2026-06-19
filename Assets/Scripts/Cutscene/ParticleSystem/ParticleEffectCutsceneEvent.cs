using UnityEngine;

public class ParticleEffectCutsceneEvent : BaseCutsceneEvent
{
    private GameObject _particleSystem;

    public override void SetUpContent(SO_CutSceneEventContent content)
    {
        base.SetUpContent(content);
    }

    public override void EndEvent()
    {
        _particleSystem = null;
        NotifyFinished();
    }

    public override void ExecuteEvent()
    {
        if (_particleSystem == null)
        {
            EndEvent();
            return;
        }

        _particleSystem.SetActive(true);
        EndEvent();
    }

    public void SetParticleSystem(GameObject system) => _particleSystem = system;
}
