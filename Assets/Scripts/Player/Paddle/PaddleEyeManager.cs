using FMOD.Studio;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public enum EMOTION
{
    NORMAL,
    PLEASE,
    JOY,
    EXCITED,
    ESTATIC,
}
[System.Serializable]
public struct EyePerformance
{
    public int comboThreshold;
    public Sprite _eyeBackLeft;
    public Sprite _eyeBackRight;
    public Sprite _leftPupil;
    public Sprite _rightPupil;
    public Sprite _mouth;
}


public class PaddleEyeManager : MonoBehaviour
{

    Ball _ballManager;


    public GameObject [] _paddleEyes = new GameObject [2];

    [Header("Blink Settings")]
    [SerializeField] float _minBlinkInterval = 2f;
    [SerializeField] float _maxBlinkInterval = 5f;
    [SerializeField] float _blinkDuration = 0.1f;
    public bool _stopBlinking;

    Coroutine _blinkRoutine;
    Coroutine _forcedBlinkRoutine;

    [Header("Eye Emotion")]
    [SerializeField] SpriteRenderer _paddleLeftPupil;
    [SerializeField] SpriteRenderer _paddleRightPupil;
    [SerializeField] SpriteRenderer _paddleLeftEyeBack;
    [SerializeField] SpriteRenderer _paddleRightEyeBack;
    [SerializeField] EyePerformance[] _emotionPerformance;
    [SerializeField] SpriteRenderer _mouth;
    EMOTION _currentEmotion;


    private void Awake()
    {
        _ballManager = FindAnyObjectByType<Ball>();
    }

    void Start()
    {


        _ballManager.OnBrickHit += ChangePupil;
        _ballManager.OnBallReset += ChangePupil;

        _blinkRoutine = StartCoroutine(BlinkRoutine());
        ChangePupil();
    }
    private void OnDestroy()
    {
        _ballManager.OnBrickHit -= ChangePupil;
        _ballManager.OnBallReset -= ChangePupil;
    }

    IEnumerator BlinkRoutine()
    {
        if (_stopBlinking)
            yield return null;

        while (true)
        {
            float waitTime = Random.Range(_minBlinkInterval, _maxBlinkInterval);
            yield return new WaitForSeconds(waitTime);

            yield return BlinkOnce();
        }
    }

    public void BlinkNow()
    {
        // Prevent overlapping forced blinks
        if (_forcedBlinkRoutine != null)
            StopCoroutine(_forcedBlinkRoutine);

        _forcedBlinkRoutine = StartCoroutine(BlinkOnce());
    }
    public void DoubleBlink()
    {
        StartCoroutine(DoubleBlinkRoutine());
    }
    public void CloseEye()
    {
        StopAllCoroutines();
        SetEyesActive(false);
        _stopBlinking = true;
    }


    public void OpenEye() 
    {
        SetEyesActive(true);
        _stopBlinking = false;
        _blinkRoutine = StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkOnce()
    {
        if (_stopBlinking)
            yield return null;

        SetEyesActive(false);
        yield return new WaitForSeconds(_blinkDuration);
        SetEyesActive(true);
    }
    IEnumerator DoubleBlinkRoutine()
    {
        if (_stopBlinking)
            yield return null;

        yield return BlinkOnce();
        yield return new WaitForSeconds(0.1f);
        yield return BlinkOnce();
    }

    void SetEyesActive(bool isActive)
    {
        foreach (var eye in _paddleEyes)
        {
            if (eye != null)
                eye.SetActive(isActive);
        }
    }

    public void ChangePupil()
    {

        int combo = _ballManager._currentCombo;

        for (int i = _emotionPerformance.Length - 1; i >= 0; i--)
        {
            if (combo >= _emotionPerformance[i].comboThreshold)
            {
                _paddleLeftPupil.sprite = _emotionPerformance[i]._leftPupil;
                _paddleRightPupil.sprite = _emotionPerformance[i]._rightPupil;
                _paddleLeftEyeBack.sprite = _emotionPerformance[i]._eyeBackLeft;
                _paddleRightEyeBack.sprite = _emotionPerformance[i]._eyeBackRight;
                _mouth.sprite = _emotionPerformance[i]._mouth;
                
                return;
            }
        }
    }

}
