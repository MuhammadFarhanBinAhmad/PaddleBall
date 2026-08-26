using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class GlobalFeedbackManager : MonoBehaviour
{
    BrickPool _brickPool;
    CameraShake _cameraShake;

    public Action PlayGlobalFeedback;

    [Header("PlayerObjects")]
    [SerializeField] Transform _ballTransform;
    [SerializeField] Transform _paddleTransform;

    [Header("AffectedObjects")]
    [SerializeField] List<GameObject> _wallTransforms = new List<GameObject>();
    [SerializeField] Transform[] _brickTransforms;
    [SerializeField] AnimationCurve _feedbackAnimCurve;
    float _animationDuration;
    
    [Header("FlashEffect")]
    public GameObject _flashScreen;
    public bool _playFlash;
    public float _flashDuration;

    [Header("FreezeEffect")]
    [SerializeField] bool _useFullFreeze = true;
    [SerializeField] float _slowMotionScale;
    [SerializeField] float _freezeRealtimeDuration;
    Coroutine _freezeRoutine;


    //CamShakeValue
    float _duration;
    float _trauma;

    //Starting Scale
    float _paddleStartScaleMultiplier;
    //float _ballStartScaleMultiplier;
    float _wallStartScaleMultipler;
    float _brickStartScaleMultipler;
    //float _ballEndScaleMultiplier
    float _paddleEndScaleMultiplier, _wallEndScaleMultipler, _brickEndScaleMultipler;

    Coroutine _shakeWorldRoutine;

    // cached originals
    Vector3 ballOriginalScale;
    Vector3 paddleOriginalScale;
    Vector3[] wallOriginalScales = Array.Empty<Vector3>();
    Vector3[] brickOriginalScales = Array.Empty<Vector3>();

    public static GlobalFeedbackManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
            print("more than one instance in the scene");

        Instance = this;

        _brickPool = FindAnyObjectByType<BrickPool>();
        _cameraShake = FindAnyObjectByType<CameraShake>();

        if (_ballTransform != null)
            ballOriginalScale = _ballTransform.localScale;

        List<GameObject> bricks = _brickPool.GetListOfBrick();
        _brickTransforms = new Transform[bricks.Count];

        for (int i = 0; i < bricks.Count; i++)
        {
            _brickTransforms[i] = bricks[i].transform;
        }

        // cache original scales
        if (_paddleTransform != null)
            paddleOriginalScale = _paddleTransform.localScale;

        if (ballOriginalScale != null)
            ballOriginalScale = _ballTransform.localScale;



        brickOriginalScales = new Vector3[_brickTransforms.Length];
        for (int i = 0; i < _brickTransforms.Length; i++)
            brickOriginalScales[i] = _brickTransforms[i].localScale;

        PlayGlobalFeedback += PlayShakeWorldAnimation;
        PlayGlobalFeedback += PlayCamShakeEvent;
    }

    public void SetWall(List<GameObject> wall)
    {
        _wallTransforms = wall;
        wallOriginalScales = new Vector3[_wallTransforms != null ? _wallTransforms.Count : 0];
        for (int i = 0; i < wallOriginalScales.Length; i++)
            wallOriginalScales[i] = _wallTransforms[i].transform.localScale;

    }

    private void OnDestroy()
    {
        PlayGlobalFeedback -= PlayShakeWorldAnimation;
        PlayGlobalFeedback -= PlayCamShakeEvent;

    }
    void PlayCamShakeEvent()
    {
        _cameraShake.StartShake(_duration, _trauma);
    }
    void PlayShakeWorldAnimation()
    {
        if (_shakeWorldRoutine != null)
            StopCoroutine(_shakeWorldRoutine);

        // reset scales immediately to originals before starting (helps prevent stacking scale errors)
        ResetAllScalesToOriginal();

        _shakeWorldRoutine = StartCoroutine(AnimateSizeLerpWorld());
    }
    void ResetAllScalesToOriginal()
    {
        if (_paddleTransform != null)
            _paddleTransform.localScale = paddleOriginalScale;

        if( _ballTransform != null)
            _ballTransform.localScale = ballOriginalScale;

        for (int i = 0; i < wallOriginalScales.Length; i++)
            if (_wallTransforms[i] != null)
                _wallTransforms[i].transform.localScale = wallOriginalScales[i];

        for(int i=0; i < brickOriginalScales.Length;i++)
            if(_brickTransforms[i] != null)
                _brickTransforms[i].localScale = brickOriginalScales[i];
    }

    IEnumerator AnimateSizeLerpWorld()
    {
        float time = 0f;

        // precompute targets
        Vector3 paddleTarget = paddleOriginalScale * _paddleEndScaleMultiplier;
        Vector3 paddleStart = paddleOriginalScale * _paddleStartScaleMultiplier;

        //Vector3 ballTarget = ballOriginalScale * _ballEndScaleMultiplier;
        //Vector3 ballStart = ballOriginalScale * _ballStartScaleMultiplier;

        Vector3[] wallTargets = new Vector3[wallOriginalScales.Length];
        Vector3[] wallStarts = new Vector3[wallOriginalScales.Length];

        for (int i = 0; i < wallOriginalScales.Length; i++)
        {
            wallTargets[i] = wallOriginalScales[i] * _wallEndScaleMultipler;
            wallStarts[i] = wallOriginalScales[i] * _wallStartScaleMultipler;
        }

        Vector3[] brickTargets = new Vector3[brickOriginalScales.Length];
        Vector3[] brickStarts = new Vector3[brickOriginalScales.Length];
        for (int i = 0; i < brickOriginalScales.Length; i++)
        {
            brickTargets[i] = brickOriginalScales[i] * _brickEndScaleMultipler;
            brickStarts[i] = brickOriginalScales[i] * _brickStartScaleMultipler;
        }

        while (time < _animationDuration)
        {
            float normalized = time / _animationDuration;
            float curveValue = _feedbackAnimCurve.Evaluate(normalized);

            // paddle
            if (_paddleTransform != null)
                _paddleTransform.localScale = Vector3.LerpUnclamped(paddleStart, paddleTarget, curveValue);

            // ball
            //if (_ballTransform != null)
            //    _ballTransform.localScale = Vector3.LerpUnclamped(ballStart, ballTarget, curveValue);

            // walls
            for (int i = 0; i < _wallTransforms.Count; i++)
            {
                var wt = _wallTransforms[i];
                if (wt == null) continue;
                wt.transform.localScale = Vector3.LerpUnclamped(wallStarts[i], wallTargets[i], curveValue);
            }

            //bricks
            for(int i=0; i < _brickTransforms.Length;i++)
            {
                var bt = _brickTransforms[i];
                if (bt == null || !bt.gameObject.activeInHierarchy) continue;
                bt.localScale = Vector3.LerpUnclamped(brickStarts[i], brickTargets[i], curveValue);
            }

            time += Time.deltaTime;
            yield return null;
        }

        // ensure exact final and then restore originals

        if (_ballTransform!= null)
            _ballTransform.localScale = ballOriginalScale;

        if (_paddleTransform != null)
            _paddleTransform.localScale = paddleOriginalScale;

        for (int i = 0; i < _wallTransforms.Count; i++)
            if (_wallTransforms[i] != null)
                _wallTransforms[i].transform.localScale = wallOriginalScales[i];

        for(int i =0;i < _brickTransforms.Length;i++)
            if (_brickTransforms[i] != null)
                _brickTransforms[i].localScale = brickOriginalScales[i];

        _shakeWorldRoutine = null;
    }
    public void SetFeedbackValue(SO_FeedbackEffect _fe)
    {
        _feedbackAnimCurve = _fe._animCurve;

        _duration = _fe._duration;
        _trauma = _fe._trauma;

        _animationDuration = _fe.animationDuration;

        _paddleStartScaleMultiplier = _fe._startscaleMultiplier;
        //_ballStartScaleMultiplier = so_OnBrickDestroy._startscaleMultiplier;
        _wallStartScaleMultipler = _fe._startscaleMultiplier;
        _brickStartScaleMultipler = _fe._startscaleMultiplier;

        //_ballEndScaleMultiplier = so_OnBrickDestroy._endscaleMultiplier;
        _paddleEndScaleMultiplier = _fe._endscaleMultiplier;
        _wallEndScaleMultipler = _fe._endscaleMultiplier;
        _brickEndScaleMultipler = _fe._endscaleMultiplier;
    }
    public void PlayFreezeFrame()
    {
        if (_freezeRoutine != null)
            StopCoroutine(_freezeRoutine);

        _freezeRoutine = StartCoroutine(FreezeFrameRoutine());
    }
    IEnumerator FreezeFrameRoutine()
    {
        // Save original time values
        float originalTimeScale = 1;
        float originalFixedDelta = Time.fixedDeltaTime;

        // Choose target timescale: either full freeze (0) or small slow-mo
        float targetScale = _useFullFreeze ? 0f : Mathf.Clamp(_slowMotionScale, 0.0001f, 1f);

        // Apply freeze/slowdown
        Time.timeScale = targetScale;
        // Adjust fixedDelta to keep physics consistent when timeScale changes
        Time.fixedDeltaTime = originalFixedDelta * Time.timeScale;

        // Wait in real time (unaffected by Time.timeScale)
        yield return new WaitForSecondsRealtime(_freezeRealtimeDuration);

        // Restore original values
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDelta;

        _freezeRoutine = null;

        //Flash
        if (_playFlash)
            _flashScreen.SetActive(true);
        else
            yield return null;

        yield return new WaitForSecondsRealtime(_flashDuration);
        _flashScreen.SetActive(false);
    }
}
