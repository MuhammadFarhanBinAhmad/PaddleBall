using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TowerEssence : MonoBehaviour
{
    TowerManager _towerManager;
    PaddleVacoom _paddleVacoom;

    [Header("Essence Stats")]
    [SerializeField] int _essenceMinWorth;
    [SerializeField] int _essenceMaxWorth;
    [SerializeField] float _essenceBonusMultiplier;
    [SerializeField] float[] _essenceExpirationTime;
    [SerializeField] float _essenceCurrentLiveTime;
    int _currentExpirationPhase;
    Action _OnCollectionEvent;

    [Header("Light")]
    [SerializeField] Light2D _light2D;
    [SerializeField] float _startIntensity;
    [SerializeField] float _endIntensity;

    float intensityTimer;
    bool intensityDone;

    [SerializeField] float intensityDuration = 3f;

    [Header("Particle Effects")]
    [SerializeField] ParticleSystem _particleEffects;

    [Header("Movement")]
    [Tooltip("Maximum movement speed.")]
    public float maxSpeed = 10f;

    [Tooltip("Movement drag while being attracted.")]
    public float attractedDrag = 5f;

    [Tooltip("Normal movement drag.")]
    public float normalDrag = 1f;

    [Tooltip("Starting movement impulse.")]
    public float minImpulse;

    public float maxImpulse;

    Vector2 _velocity;

    [Header("Suction / Collection")]
    public float _collectDistance;

    [Header("Suction Tuning")]
    [Tooltip("How strongly the Essence accelerates toward the vacuum.")]
    [SerializeField] float _attractStrength;
    [SerializeField]float _attractRadius = 1f;

    [Tooltip("How much velocity is retained while being sucked.")]
    [Range(0f, 1f)]
    public float suctionDragMultiplier = 0.9f;

    // Attraction state
    bool _isAttracted;
    Vector2 _attractorPos;
    Transform _attractorTransform;



    [Header("Auto Attract")]
    [SerializeField] float _autoAttractStrength = 8f;
    [SerializeField] float _autoAttractRadius = 999f;
    [SerializeField] bool _autoAttract;

    void Awake()
    {
        _towerManager = FindAnyObjectByType<TowerManager>();
        _paddleVacoom = FindAnyObjectByType<PaddleVacoom>();
    }

    private void OnEnable()
    {
        // Reset movement state.
        _velocity = Vector2.zero;

        // Reproduce the old initial Rigidbody impulse
        // using our own velocity instead.
        Vector2 direction = UnityEngine.Random.insideUnitCircle;

        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector2.up;

        direction.Normalize();

        float magnitude =
            UnityEngine.Random.Range(minImpulse, maxImpulse);

        _velocity = direction * magnitude;

        // Reset visual state.
        intensityTimer = 0f;
        intensityDone = false;

        _light2D.intensity = _startIntensity;
    }

    private void OnDisable()
    {
        _OnCollectionEvent = null;
    }

    public void SetToAutoAttract()
    {
        _autoAttract = true;
    }

    private void Update()
    {
        UpdateLifetime();
        UpdateLight();

        if (_isAttracted)
        {
            UpdateAttraction();
        }

        Move();

        CheckCollectionDistance();
    }

    void UpdateLifetime()
    {
        _essenceCurrentLiveTime += Time.deltaTime;

        if (_currentExpirationPhase >= _essenceExpirationTime.Length)
            return;

        if (_essenceCurrentLiveTime >
            _essenceExpirationTime[_currentExpirationPhase])
        {
            _currentExpirationPhase++;

            if (_currentExpirationPhase == 1)
            {
                _particleEffects.Stop();
            }

            if (_currentExpirationPhase == 3)
            {
                ResetStats();
            }
        }
    }

    void UpdateLight()
    {
        if (intensityDone)
            return;

        intensityTimer += Time.deltaTime;

        float time =
            Mathf.Clamp01(intensityTimer / intensityDuration);

        _light2D.intensity =
            Mathf.Lerp(_startIntensity, _endIntensity, time);

        if (time >= 1f)
        {
            intensityDone = true;
        }
    }

    void UpdateAttraction()
    {
        // Auto-attract override.
        if (_autoAttract)
        {
            _attractorPos = _paddleVacoom.transform.position;
            _attractStrength = _autoAttractStrength;
            _attractRadius = _autoAttractRadius;
        }
        else if (_attractorTransform != null)
        {
            _attractorPos = _attractorTransform.position;
        }

        Vector2 toTarget =
            _attractorPos - (Vector2)transform.position;

        float distanceSqr = toTarget.sqrMagnitude;

        if (distanceSqr <= 0.0001f)
            return;

        float distance = Mathf.Sqrt(distanceSqr);

        if (distance > _attractRadius)
        {
            if (!_autoAttract)
            {
                StopAttraction();
                return;
            }
        }

        float t =
            Mathf.Clamp01(
                1f - (distance / _attractRadius)
            );

        float pull =
            _attractStrength *
            (t * 0.9f + 0.1f);

        Vector2 direction =
            toTarget / distance;

        // Accelerate toward the target.
        _velocity +=
            direction *
            pull *
            Time.deltaTime;

        // Apply suction drag.
        _velocity *=
            Mathf.Pow(
                suctionDragMultiplier,
                Time.deltaTime * 60f
            );

        // Limit velocity.
        if (_velocity.sqrMagnitude >
            maxSpeed * maxSpeed)
        {
            _velocity =
                _velocity.normalized * maxSpeed;
        }
    }

    void Move()
    {
        if (_velocity.sqrMagnitude <= 0.000001f)
            return;

        // Apply normal drag when not being sucked.
        if (!_isAttracted)
        {
            _velocity *=
                Mathf.Pow(
                    normalDrag,
                    Time.deltaTime
                );
        }

        transform.position +=
            (Vector3)(_velocity * Time.deltaTime);
    }

    void CheckCollectionDistance()
    {
        if (_paddleVacoom == null)
            return;

        Vector2 delta =
            (Vector2)transform.position -
            (Vector2)_paddleVacoom.transform.position;

        if (delta.sqrMagnitude <=
            _collectDistance * _collectDistance)
        {
            HandleCollection();
        }
    }

    // -------------------------
    // Suction API
    // -------------------------

    public void StartAttraction(
        Transform targetTransform,
        float strength,
        float radius)
    {
        _attractorTransform = targetTransform;
        _attractorPos = targetTransform.position;

        _attractRadius =
            Mathf.Max(0.01f, radius);

        _attractStrength = strength;

        _isAttracted = true;
    }

    public void UpdateAttractionTarget(
        Vector2 targetPosition)
    {
        _attractorPos = targetPosition;
    }

    public void StopAttraction()
    {
        _isAttracted = false;
        _attractorTransform = null;
    }

    // -------------------------
    // Collection
    // -------------------------

    public void HandleCollection()
    {
        AudioManager.Instance.PlayOneShot(
            FmodEvent.Instance.sfx_essenceCollect,
            transform.position
        );

        switch (_currentExpirationPhase)
        {
            case 0:
                _towerManager.IncreaseEssenceCount(
                    GetBonusEssence()
                );
                break;

            case 1:
                _towerManager.IncreaseEssenceCount(
                    GetNormalEssence()
                );
                break;

            case 2:
                _towerManager.IncreaseEssenceCount(
                    GetHalfEssence()
                );
                break;
        }

        ResetStats();
    }

    void ResetStats()
    {
        _velocity = Vector2.zero;

        _currentExpirationPhase = 0;
        _essenceCurrentLiveTime = 0f;

        _particleEffects.Play();

        _autoAttract = false;

        StopAttraction();

        gameObject.SetActive(false);
    }

    // -------------------------
    // Essence Value
    // -------------------------

    int GetBonusEssence()
    {
        int essence =
            UnityEngine.Random.Range(
                _essenceMinWorth,
                _essenceMaxWorth
            );

        return (int)(
            essence *
            _essenceBonusMultiplier
        );
    }

    int GetNormalEssence()
    {
        int essence =
                    UnityEngine.Random.Range(
                        _essenceMinWorth,
                        _essenceMaxWorth
                    );

        return 
            essence
        ;
    }

    int GetHalfEssence()
    {
        int essence =
                    UnityEngine.Random.Range(
                        _essenceMinWorth,
                        _essenceMaxWorth
                    );

        return (int)(
            essence * .5f);
    }
}