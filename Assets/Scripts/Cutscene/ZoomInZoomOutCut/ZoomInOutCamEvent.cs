
using System.Collections;
using Cinemachine;
using UnityEngine;

public class ZoomInOutCamEvent : BaseCutsceneEvent
{
    [Header("Camera References")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    Transform _zoomTarget;

    [Header("Zoom Settings")]
    [SerializeField, Min(0.01f)]
    float _zoomSize = 3f;

    [SerializeField, Min(0f)]
    float _zoomInDuration = 1f;

    [SerializeField, Min(0f)]
    float _holdDuration = 2f;

    [SerializeField, Min(0f)]
    float _zoomOutDuration = 1f;

    [Header("Zoom Curve")]
    [SerializeField]
    AnimationCurve _zoomCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Camera Breathing Effect")]
    [SerializeField, Min(0f)]
    float _breathAmplitude = 0.15f;
    [SerializeField, Min(0f)]
    float _breathFrequency = 0.5f;

    Coroutine _zoomRoutine;

    Transform _originalPosition;
    Transform _originalFollow;
    Transform _originalLookAt;
    Transform _temporaryFocus;

    float _originalZoomSize;

    bool _isExecuting;

    public void SetTarget(Transform target)
    {
        _zoomTarget = target;
        _originalPosition = _virtualCamera.transform;
        _virtualCamera.Follow = _zoomTarget;
        _virtualCamera.LookAt = _zoomTarget;


        _originalFollow = _virtualCamera.Follow;
        _originalLookAt = _virtualCamera.LookAt;
        _originalZoomSize = GetZoomSize();
    }
    public override void ExecuteEvent()
    {
        if (_isExecuting)
            return;

        if (_virtualCamera == null || _zoomTarget == null)
        {
            Debug.LogError(
                $"{name}: Missing camera or zoom target."
            );

            NotifyFinished();
            return;
        }

        if (_virtualCamera.Follow == null)
        {
            Debug.LogError(
                $"{name}: Cinemachine camera needs a Follow target."
            );

            NotifyFinished();
            return;
        }

        // Save original camera settings.


        // Create a temporary object for smooth camera movement.
        GameObject focusObject = new GameObject(
            "Cutscene Camera Focus"
        );

        _temporaryFocus = focusObject.transform;
        _temporaryFocus.position = _originalFollow.position;

        // Have Cinemachine follow our temporary focus.
        _virtualCamera.Follow = _temporaryFocus;

        if (_originalLookAt == _originalFollow)
            _virtualCamera.LookAt = _temporaryFocus;

        _isExecuting = true;

        _zoomRoutine = StartCoroutine(ZoomSequence());
    }


    private IEnumerator ZoomSequence()
    {
        // Step 1: Zoom in and move toward target.
        yield return StartCoroutine(
            AnimateCamera(
                _zoomTarget,
                _zoomSize,
                _zoomInDuration
            )
        );

        // Step 2: Hold and apply breathing effect.
        float timer = 0f;

        while (timer < _holdDuration)
        {
            timer += Time.unscaledDeltaTime;

            // Continue following the target.
            if (_zoomTarget != null && _temporaryFocus != null)
            {
                _temporaryFocus.position = _zoomTarget.position;
            }

            // Calculate breathing oscillation.
            float breath = Mathf.Sin(
                timer * Mathf.PI * 2f * _breathFrequency
            );

            // Smoothly fade breathing in and out
            // during the hold duration.
            float progress = Mathf.Clamp01(
                timer / Mathf.Max(0.001f, _holdDuration)
            );

            float breathWeight = Mathf.Sin(
                progress * Mathf.PI
            );

            // Apply breathing to orthographic size.
            float breathingZoom =
                _zoomSize +
                breath * _breathAmplitude * breathWeight;

            SetZoomSize(Mathf.Max(0.01f, breathingZoom));

            yield return null;
        }

        // Reset exact zoom value before zooming out.
        SetZoomSize(_zoomSize);

        // Step 3: Return to original camera view.
        yield return StartCoroutine(
            AnimateCamera(
                _originalFollow,
                _originalZoomSize,
                _zoomOutDuration
            )
        );

        _virtualCamera.transform.position = _originalPosition.position;

        // Step 4: Finish the event.
        FinishEvent();
    }


    private IEnumerator AnimateCamera(
        Transform target,
        float targetZoom,
        float duration)
    {
        if (target == null || _temporaryFocus == null)
            yield break;

        Vector3 startPosition = _temporaryFocus.position;
        float startZoom = GetZoomSize();

        if (duration <= 0f)
        {
            _temporaryFocus.position = target.position;
            SetZoomSize(targetZoom);
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float easedT = _zoomCurve.Evaluate(t);

            // Smoothly move camera focus.
            _temporaryFocus.position = Vector3.Lerp(
                startPosition,
                target.position,
                easedT
            );

            // Smoothly change orthographic zoom.
            float currentZoom = Mathf.Lerp(
                startZoom,
                targetZoom,
                easedT
            );

            SetZoomSize(currentZoom);

            yield return null;
        }

        // Ensure final values are exact.
        _temporaryFocus.position = target.position;
        SetZoomSize(targetZoom);
    }

    private float GetZoomSize()
    {
        return _virtualCamera.m_Lens.OrthographicSize;
    }

    private void SetZoomSize(float size)
    {
        var lens = _virtualCamera.m_Lens;
        lens.OrthographicSize = size;
        _virtualCamera.m_Lens = lens;
    }

    private void FinishEvent()
    {
        if (!_isExecuting)
            return;

        _isExecuting = false;

        // Restore original camera settings.
        _virtualCamera.Follow = _originalFollow;
        _virtualCamera.LookAt = _originalLookAt;
        SetZoomSize(_originalZoomSize);

        if (_temporaryFocus != null)
        {
            Destroy(_temporaryFocus.gameObject);
            _temporaryFocus = null;
        }

        _zoomRoutine = null;

        // Tell CutSceneManager to continue.
        NotifyFinished();
    }

    public override void EndEvent()
    {
        if (!_isExecuting)
            return;

        if (_zoomRoutine != null)
        {
            StopCoroutine(_zoomRoutine);
            _zoomRoutine = null;
        }

        FinishEvent();
    }

    private void OnDisable()
    {
        if (_isExecuting)
            EndEvent();
    }
}
