using UnityEngine;
using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;

public class CameraShake : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera _vcam;

    [Header("Shake Settings")]
    [SerializeField] private float _transformShakeStrength = 0.5f;
    [SerializeField] private float _maxAmplitude = 1f;
    [SerializeField] private float _maxRotationAngle = 10f;
    [SerializeField] private float _frequency = 10f;

    [Header("Deterministic")]
    [SerializeField] private int _shakeSeed = 1337;

    [SerializeField] AnimationCurve _shakeFalloff;

    private CinemachineBasicMultiChannelPerlin _noise;
    private Vector3 _originalLocalPos;

    private Coroutine _shakeRoutine;

    private float _seedX;
    private float _seedY;
    private float _seedRot;

    private void Awake()
    {
        if (_vcam == null)
            _vcam = GetComponent<CinemachineVirtualCamera>();

        _noise = _vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        _originalLocalPos = transform.localPosition;

        // Fixed deterministic seeds
        _seedX = _shakeSeed * 0.37f;
        _seedY = _shakeSeed * 0.73f;
        _seedRot = _shakeSeed * 1.17f;

    }
    private void Start()
    {
        if (_shakeRoutine != null)
            StopCoroutine(_shakeRoutine);
        _shakeRoutine = null;
    }
    private void OnDestroy()
    {
        if (_shakeRoutine != null)
            StopCoroutine(_shakeRoutine);
    }
    public void StartShake(float duration, float intensity = 1f)
    {
        if (_shakeRoutine != null)
            StopCoroutine(_shakeRoutine);

        _shakeRoutine = StartCoroutine(ShakeRoutine(duration, intensity));
    }

    IEnumerator ShakeRoutine(float duration, float intensity)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float normalized = elapsed / duration;

            // Optional fade out
            float strength = intensity * _shakeFalloff.Evaluate(normalized);

            float sampleTime = elapsed * _frequency;

            //--------------------------
            // Cinemachine noise
            //--------------------------
            if (_noise != null)
            {
                _noise.m_AmplitudeGain = _maxAmplitude * strength;
                _noise.m_FrequencyGain = _frequency;
            }

            //--------------------------
            // Rotation shake
            //--------------------------
            float rotNoise =
                (Mathf.PerlinNoise(_seedRot, sampleTime) - 0.5f) * 2f;

            _vcam.m_Lens.Dutch =
                rotNoise * _maxRotationAngle * strength;

            //--------------------------
            // Position shake
            //--------------------------
            float x =
                (Mathf.PerlinNoise(_seedX, sampleTime) - 0.5f) * 2f;

            float y =
                (Mathf.PerlinNoise(_seedY, sampleTime) - 0.5f) * 2f;

            Vector3 offset =
                new Vector3(x, y, 0f)
                * _transformShakeStrength
                * strength;

            transform.localPosition = _originalLocalPos + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        ResetShake();

        _shakeRoutine = null;
    }

    void ResetShake()
    {
        if (_noise != null)
        {
            _noise.m_AmplitudeGain = 0f;
            _noise.m_FrequencyGain = 0f;
        }

        if (_vcam != null)
            _vcam.m_Lens.Dutch = 0f;

        transform.localPosition = _originalLocalPos;
    }
}