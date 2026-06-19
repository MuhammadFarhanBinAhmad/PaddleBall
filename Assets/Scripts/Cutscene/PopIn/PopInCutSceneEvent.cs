using System;
using System.Collections;
using UnityEngine;

public class PopInCutSceneEvent : BaseCutsceneEvent
{
    GameObject _targetObject;

    private Coroutine _routine;
    private Vector3 _originalScale;
    private bool _hasEnded;

    public override void SetUpContent(SO_CutSceneEventContent content)
    {
        base.SetUpContent(content);
        if (_targetObject == null)
            _targetObject = gameObject;

        _originalScale = _targetObject.transform.localScale;
        _targetObject.SetActive(false);
        _hasEnded = false;
    }
    public override void ExecuteEvent()
    {
        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(PlayPopIn());
    }

    private IEnumerator PlayPopIn()
    {
        if (_content == null || _targetObject == null)
            yield break;

        _targetObject.SetActive(true);

        Vector3 startScale = _originalScale * _content._startScaleMultiplier;
        Vector3 endScale = _originalScale * _content._endScaleMultiplier;

        _targetObject.transform.localScale = startScale;

        float time = 0f;
        while (time < _content._animationPopInDuration)
        {
            float t = time / _content._animationPopInDuration;
            float curveT = _content._popInEffectLerp != null
                ? _content._popInEffectLerp.Evaluate(t)
                : t;

            _targetObject.transform.localScale =
                Vector3.LerpUnclamped(startScale, endScale, curveT);

            time += Time.deltaTime;
            yield return null;
        }

        _targetObject.transform.localScale = endScale;
        EndEvent();
    }
    public void SetTarget(GameObject target) => _targetObject = target;
    public override void EndEvent()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        NotifyFinished();

    }
}
