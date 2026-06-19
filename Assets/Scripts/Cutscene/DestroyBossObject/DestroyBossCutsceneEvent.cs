using System.Collections;
using UnityEngine;

public class DestroyBossCutsceneEvent : BaseCutsceneEvent
{
    GameObject _targetObject;
    float _delayTimer;

    public override void SetUpContent(SO_CutSceneEventContent content)
    {
        base.SetUpContent(content);
        _delayTimer = content._timeDelay;
    }

    public override void EndEvent()
    {

        print("delay ends");

        if (_targetObject != null)
            Destroy(_targetObject);

        NotifyFinished();
    }

    IEnumerator DelayBeforeDeath()
    {
        yield return new WaitForSeconds(_delayTimer);
        EndEvent();
    }

    public override void ExecuteEvent()
    {
        StartCoroutine(DelayBeforeDeath());
    }

    public void SetTarget(GameObject target) => _targetObject = target;

}
