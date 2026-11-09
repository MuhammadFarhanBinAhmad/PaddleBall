
using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerEssence : MonoBehaviour
{
    private TowerManager _towerManager;
    private PaddleVacoom _paddleVacoom;
    private Transform _cachedTransform;

    [Header("Essence Stats")]
    [SerializeField] private int _essenceMinWorth;
    [SerializeField] private int _essenceMaxWorth;
    [SerializeField] private float _essenceBonusMultiplier;

    // Cumulative times, e.g. [5, 10, 15].
    [SerializeField]
    private float[] _essenceExpirationTime =
        { 5f, 10f, 15f };

    [SerializeField] private float _essenceCurrentLiveTime;

    private int _currentExpirationPhase;
    private float _totalLifetime;

    [Header("Essence Scaling")]
    [SerializeField, Min(0f)]
    private float _startScaleMultiplier = 1.5f;

    [SerializeField, Min(0f)]
    private float _endScaleMultiplier = 0f;

    [SerializeField]
    private AnimationCurve _shrinkCurve =
        AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Essence Pulse")]
    [SerializeField, Range(0f, 1f)]
    private float _minPulseMultiplier = 0.7f;

    [SerializeField, Min(0f)]
    private float _pulseSpeed = 2f;

    private float _pulseTimer;
    private Vector3 _originalScale;

    [Header("Movement")]
    [Tooltip("Maximum movement speed.")]
    public float maxSpeed;

    [Tooltip("Normal movement drag.")]
    public float normalDrag;

    [Tooltip("Starting movement impulse.")]
    public float minImpulse;
    public float maxImpulse;

    private Vector2 _velocity;

    [Header("Suction / Collection")]
    public float _collectDistance;

    [Header("Suction Tuning")]
    [SerializeField] private float _attractStrength;
    [SerializeField] private float _attractRadius;

    [Tooltip("How much velocity is retained while being sucked.")]
    [Range(0f, 1f)]
    public float suctionDragMultiplier;

    private bool _isAttracted;
    private Vector2 _attractorPos;
    private Transform _attractorTransform;

    [Header("Auto Attract")]
    [SerializeField] private float _autoAttractStrength;
    [SerializeField] private float _autoAttractRadius;
    [SerializeField] private bool _autoAttract;

    private bool _defaultAutoAttract;

    [Header("Gravity")]
    [SerializeField]
    private float _gravityStrength;




    [SerializeField]
    private float _maxFallSpeed;

    // Tutorial tracking.
    private static readonly HashSet<TowerEssence> _activeEssences =
        new HashSet<TowerEssence>();

    public static IEnumerable<TowerEssence> ActiveEssences =>
        _activeEssences;

    public static event Action<TowerEssence> OnEssenceSpawned;

    public event Action<TowerEssence> OnCollected;

    private void Awake()
    {
        _cachedTransform = transform;
        _originalScale = _cachedTransform.localScale;

        _towerManager = FindAnyObjectByType<TowerManager>();
        _paddleVacoom = FindAnyObjectByType<PaddleVacoom>();

        _defaultAutoAttract = _autoAttract;

        // Cache expiration duration instead of calculating it
        // every Update.
        if (_essenceExpirationTime != null &&
            _essenceExpirationTime.Length > 0)
        {
            _totalLifetime = _essenceExpirationTime[
                _essenceExpirationTime.Length - 1
            ];
        }
    }

    private void OnEnable()
    {
        // Reset all state when reused by the object pool.
        _essenceCurrentLiveTime = 0f;
        _currentExpirationPhase = 0;

        _isAttracted = false;
        _attractorTransform = null;
        _attractorPos = Vector2.zero;
        _autoAttract = _defaultAutoAttract;

        _velocity = Vector2.zero;
        _pulseTimer = 0f;

        _cachedTransform.localScale =
            _originalScale * _startScaleMultiplier;
        // Initial impulse.
        Vector2 direction = UnityEngine.Random.insideUnitCircle;

        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector2.up;

        direction.Normalize();

        float magnitude = UnityEngine.Random.Range(
            minImpulse,
            maxImpulse
        );

        _velocity = direction * magnitude;

        // Reset to the starting scale.
        _cachedTransform.localScale =
            _originalScale * _startScaleMultiplier;

        _activeEssences.Add(this);
        OnEssenceSpawned?.Invoke(this);
    }

    private void OnDisable()
    {
        _activeEssences.Remove(this);

        _isAttracted = false;
        _attractorTransform = null;
        _velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        float deltaTime = Time.deltaTime;

        // Stop processing if Essence has expired.
        if (UpdateLifetime(deltaTime))
            return;

        UpdateScale();

        if (_isAttracted || _autoAttract)
            UpdateAttraction(deltaTime);

        Move(deltaTime);
    }

    // -------------------------
    // Lifetime
    // -------------------------

    private bool UpdateLifetime(float deltaTime)
    {
        _essenceCurrentLiveTime += deltaTime;

        if (_essenceExpirationTime == null ||
            _essenceExpirationTime.Length == 0)
        {
            return false;
        }

        // Supports passing multiple expiration thresholds
        // in a single frame.
        while (_currentExpirationPhase <
               _essenceExpirationTime.Length &&
               _essenceCurrentLiveTime >=
               _essenceExpirationTime[_currentExpirationPhase])
        {
            _currentExpirationPhase++;

            // Existing collection rewards use phases 0, 1, 2.
            // Expire after the final phase threshold.
            if (_currentExpirationPhase >= 3 ||
                _currentExpirationPhase >=
                _essenceExpirationTime.Length)
            {
                ResetStats();
                return true;
            }
        }

        return false;
    }

    // -------------------------
    // Scaling
    // -------------------------


    private void UpdateScale()
    {
        if (_totalLifetime <= 0f)
            return;

        // Step 1: Calculate how much lifetime has passed.
        float lifetimeProgress = Mathf.Clamp01(
            _essenceCurrentLiveTime / _totalLifetime
        );

        // Step 2: Calculate the current maximum size.
        float shrinkProgress = Mathf.Clamp01(
            _shrinkCurve.Evaluate(lifetimeProgress)
        );

        float currentMaxScale = Mathf.Lerp(
            _startScaleMultiplier,
            _endScaleMultiplier,
            shrinkProgress
        );

        // Step 3: Calculate ping-pong effect.
        _pulseTimer += Time.deltaTime;

        float pulse = Mathf.PingPong(
            _pulseTimer * _pulseSpeed,
            1f
        );

        // Step 4: Calculate size relative to current maximum.
        float pulseMultiplier = Mathf.Lerp(
            _minPulseMultiplier,
            1f,
            pulse
        );

        // Step 5: Apply final scale.
        float finalScale = currentMaxScale * pulseMultiplier;

        _cachedTransform.localScale =
            _originalScale * finalScale;
    }


    // -------------------------
    // Attraction
    // -------------------------

    public void SetToAutoAttract()
    {
        _autoAttract = true;
        _isAttracted = true;
    }

    private void UpdateAttraction(float deltaTime)
    {
        float strength = _attractStrength;
        float radius = _attractRadius;

        if (_autoAttract)
        {
            if (_paddleVacoom == null)
                return;

            _attractorPos = _paddleVacoom.transform.position;

            strength = _autoAttractStrength;
            radius = _autoAttractRadius;
        }
        else if (_attractorTransform != null)
        {
            _attractorPos = _attractorTransform.position;
        }

        radius = Mathf.Max(0.01f, radius);

        Vector2 toTarget =
            _attractorPos - (Vector2)_cachedTransform.position;

        float distanceSqr = toTarget.sqrMagnitude;

        if (distanceSqr <= 0.0001f)
            return;

        float distance = Mathf.Sqrt(distanceSqr);

        if (distance > radius && !_autoAttract)
        {
            StopAttraction();
            return;
        }

        float t = Mathf.Clamp01(
            1f - distance / radius
        );

        float pull = strength * (t * 0.9f + 0.1f);

        Vector2 direction = toTarget / distance;

        _velocity += direction * pull * deltaTime;

        _velocity *= Mathf.Pow(
            suctionDragMultiplier,
            deltaTime * 60f
        );

        float maxSpeedSqr = maxSpeed * maxSpeed;

        if (_velocity.sqrMagnitude > maxSpeedSqr &&
            maxSpeed > 0f)
        {
            _velocity = _velocity.normalized * maxSpeed;
        }
    }

    // -------------------------
    // Movement
    // -------------------------

    private void Move(float deltaTime)
    {
        if (!_isAttracted && !_autoAttract)
        {
            // Gravity must continue even if velocity is zero.
            _velocity.y -= _gravityStrength * deltaTime;

            _velocity.y = Mathf.Max(
                _velocity.y,
                -_maxFallSpeed
            );

            _velocity *= Mathf.Pow(
                Mathf.Max(0f, normalDrag),
                deltaTime
            );
        }

        if (_velocity.sqrMagnitude <= 0.000001f)
            return;

        _cachedTransform.position +=
            (Vector3)(_velocity * deltaTime);
    }

    // -------------------------
    // Suction API
    // -------------------------

    public void StartAttraction(
        Transform targetTransform,
        float strength,
        float radius)
    {
        if (targetTransform == null)
            return;

        _attractorTransform = targetTransform;
        _attractorPos = targetTransform.position;

        _attractRadius = Mathf.Max(0.01f, radius);
        _attractStrength = strength;

        _isAttracted = true;
    }

    public void UpdateAttractionTarget(Vector2 targetPosition)
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
        // Prevent duplicate collection attempts.
        if (!isActiveAndEnabled)
            return;

        AudioManager.Instance.PlayOneShot(
            FmodEvent.Instance.sfx_essenceCollect,
            _cachedTransform.position
        );

        int value = 0;

        switch (_currentExpirationPhase)
        {
            case 0:
                value = GetBonusEssence();
                break;

            case 1:
                value = GetNormalEssence();
                break;

            case 2:
                value = GetHalfEssence();
                break;
        }

        _towerManager.IncreaseEssenceCount(value);

        // Notify tutorial listeners before returning to pool.
        OnCollected?.Invoke(this);

        ResetStats();
    }

    private void ResetStats()
    {
        _velocity = Vector2.zero;

        _currentExpirationPhase = 0;
        _essenceCurrentLiveTime = 0f;

        _autoAttract = false;

        StopAttraction();

        gameObject.SetActive(false);
    }

    // -------------------------
    // Essence Value
    // -------------------------

    private int GetRandomEssence()
    {
        return UnityEngine.Random.Range(
            _essenceMinWorth,
            _essenceMaxWorth
        );
    }

    private int GetBonusEssence()
    {
        return (int)(
            GetRandomEssence() * _essenceBonusMultiplier
        );
    }

    private int GetNormalEssence()
    {
        return GetRandomEssence();
    }

    private int GetHalfEssence()
    {
        return (int)(GetRandomEssence() * 0.5f);
    }
}
