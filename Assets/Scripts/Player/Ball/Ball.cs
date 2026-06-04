using System;
using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    internal Rigidbody2D _rigidbody;
    SpriteRenderer _spriteRenderer;
    CircleCollider2D _circleCollider;
    [SerializeField]ParticleSystem _deathEffect;
    [SerializeField] TrailRenderer _trailRenderer;
    AbilityManager _abilityManager;
    BrickPool _brickPool;
    BallFeedbackManager _ballFeedbackManager;
    BallDirectionArrow _ballDirectionArrow;
    PaddleHealth _paddleHealth;


    public float _gravityScale;
    public float _maxVelocity;

    public Action OnBallHit;
    public Action OnBallReset;
    public Action OnBallDestroy;//For copy
    public Action OnBallRediect;
    public Action OnBrickHit;

    public bool IsAwaitingLaunch => _awaitingLaunch;
    [SerializeField] ParticleSystem _reviveParticle;

    [Header("Damage")]
    [SerializeField] int _baseDamage;
    [SerializeField] float _camShakeStrength;


    [Header("Combo")]
    [SerializeField] int _feverThreshold;
    internal int _currentCombo;
    [SerializeField] GameObject _particleTrail;

    [Header("CopyBall")]
    public int _maxBounce;
    internal bool _copyBall;
    int _currentBounce;

    [Header("Homing (subtle)")]
    public float _delayTimeAfterHit;
    public float _currentDelayTime;
    [Range(0f, 1f)]
    [SerializeField] float _homingStrength;
    [SerializeField] float _minVerticalForHoming;
    [SerializeField] float _homingMaxDistance;

    [Header("AimingState")]
    [SerializeField] float _lerpSpeed;
    [SerializeField] float _slowMotionTimeValue;
    [SerializeField] float _coolDownPeriod;
    [SerializeField] int _manaShootCost;
    float _currentTimeScale = 1f;
    float _targetTimeScale = 1f;
    float _currentTargetTimeScale;
    float _currentCoolDownPeriod;
    Coroutine _timeScaleRoutine;
    [SerializeField] bool _onAimingState;
    [SerializeField] ParticleSystem _shotParticle;

    [Header("ManaBar")]
    [SerializeField] float _maxManaAmount;
    [SerializeField] float _currentManaAmount;
    [SerializeField] float _manaRegenRate;

    [Header("Respawn")]
    public float _respawnTime;
    public float _currentRespawnTimer;
    public Transform _respawnPos;
    [SerializeField] float _launchSpeed;
    bool _awaitingLaunch = true;
    bool _isBallDead;

    [Header("RespawnAnimation")]
    [SerializeField] AnimationCurve easeOutElastic;
    [SerializeField] float animationDuration;
    [SerializeField] float _capscaleMultiplier;
    [SerializeField] Camera _gameCamera;
    Vector3 _startingScale;

    // -------------------------
    // Push lock (prevents immediate re-attraction)
    // -------------------------
    float pushLockTimer = 0f; // seconds remaining where attraction is ignored

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _abilityManager = FindAnyObjectByType<AbilityManager>();
        _brickPool = FindAnyObjectByType<BrickPool>();
        _ballFeedbackManager = FindAnyObjectByType<BallFeedbackManager>();
        _ballDirectionArrow = FindAnyObjectByType<BallDirectionArrow>();
        _paddleHealth = FindAnyObjectByType<PaddleHealth>();

        OnBrickHit += IncreaseCombo;
        OnBrickHit += _ballFeedbackManager.UpdateGlowIntensity;

        OnBallReset += ResetCombo;
        OnBallReset += ResetBallRespawnTimer;
        OnBallReset += PlayPaddleDeathEffect;
        OnBallReset += DeactivateBall;
        //OnBallReset += ResetPosition;
        OnBallReset += PlayBallDestroyAudio;

        OnBallDestroy += DestroyCopyBall;
        OnBallDestroy += PlayBallDestroyAudio;

        OnBallRediect += RedirectBallToMouse;
        OnBallRediect += PlayBallRedirectEffect;
        OnBallRediect += StartAnimateBallRespawn;
    }
    private void Start()
    {
        _startingScale = transform.localScale;
        _currentManaAmount = _maxManaAmount;
        _ballDirectionArrow.SetEnableArrow(true);
        SetCursorState(false);

        PrepareForLaunch(_respawnPos.position);
        StartCoroutine(AnimateBallRespawn());
    }
    private void OnDisable()
    {
        OnBrickHit -= IncreaseCombo;
        OnBrickHit -= _ballFeedbackManager.UpdateGlowIntensity;

        OnBallReset -= ResetCombo;
        OnBallReset -= ResetBallRespawnTimer;
        OnBallReset -= PlayPaddleDeathEffect;
        OnBallReset -= DeactivateBall;
        //OnBallReset -= ResetPosition;
        OnBallReset -= PlayBallDestroyAudio;

        OnBallDestroy -= DestroyCopyBall;
        OnBallDestroy -= PlayBallDestroyAudio;

        OnBallRediect -= RedirectBallToMouse;
        OnBallRediect -= PlayBallRedirectEffect;
        OnBallRediect -= StartAnimateBallRespawn;

    }


    private void Update()
    {
        if (_paddleHealth.IsPaddleDead())
        {
            TimeManager.ResetTimeScale();
            _ballDirectionArrow.SetEnableArrow(false);
            return;
        }

        if(!_isBallDead)
            return;


        if(_currentRespawnTimer >0)
        {
            _currentRespawnTimer -= Time.deltaTime;
        }
        else
        {
            ResettingBall();
            _isBallDead = false;
        }
    }

    private void FixedUpdate()
    {
        if (_rigidbody.linearVelocity.magnitude > _maxVelocity)
            _rigidbody.linearVelocity = Vector2.ClampMagnitude(_rigidbody.linearVelocity, _maxVelocity);
        HandleManaRegen();


        if (_paddleHealth.IsPaddleDead())
            return;

        if (_awaitingLaunch)
            return;

        HandleTimeScaleInput();

        if (pushLockTimer > 0f)
            pushLockTimer = Mathf.Max(0f, pushLockTimer - Time.fixedDeltaTime);

        if (_currentDelayTime < 0)
            ApplyHoming();
        else
            _currentDelayTime -= Time.deltaTime;


    }

    void HandleTimeScaleInput()
    {
        if (TimeManager.IsGamePause())
            return;

        if (Input.GetMouseButton(1) && _currentCoolDownPeriod >= _coolDownPeriod) // holding
        {
            if (!_onAimingState)
                AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onBallSlowmo, transform.position);

            _onAimingState = true;
            _targetTimeScale = _slowMotionTimeValue;
            _ballDirectionArrow.SetEnableArrow(_onAimingState);

            if (_onAimingState && Input.GetMouseButton(0))
                OnBallRediect?.Invoke();
        }
        else // released
        {
            _onAimingState = false;
            _targetTimeScale = 1f;
            _ballDirectionArrow.SetEnableArrow(_onAimingState);
            if (_currentCoolDownPeriod < _coolDownPeriod)
                _currentCoolDownPeriod += Time.deltaTime;
        }
        SetCursorState(_onAimingState);
        _currentTimeScale = Mathf.Lerp(_currentTimeScale, _targetTimeScale, Time.unscaledDeltaTime * _lerpSpeed);
        TimeManager.SetCustomTimeScale(_currentTimeScale);


    }
    void HandleManaRegen()
    {
        if (_currentManaAmount < _maxManaAmount)
            _currentManaAmount += _manaRegenRate * Time.deltaTime;

    }

    void RedirectBallToMouse()
    {
        if (_currentManaAmount < _manaShootCost)
        {
            //UI pop message
            return;
        }

        if (_gameCamera == null)
            _gameCamera = Camera.main;

        if (_gameCamera == null) return;

        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = -_gameCamera.transform.position.z;

        Vector2 mouseWorld = _gameCamera.ScreenToWorldPoint(mouseScreen);

        Vector2 direction = mouseWorld - (Vector2)transform.position;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        direction.Normalize();

        float speed = _rigidbody.linearVelocity.magnitude;
        if (speed < 0.01f)
            speed = _gravityScale;

        _rigidbody.linearVelocity = direction * speed;
        transform.up = -direction;


        _currentCoolDownPeriod = 0;
        MinusMana(_manaShootCost);
        StartCoroutine(AnimateBallRespawn());

        AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onBallShoot, transform.position);

    }
    public void StartAnimateBallRespawn()
    {
        StartCoroutine(AnimateBallRespawn());
    }
    public void PlayBallRedirectEffect()
    {
        if (_shotParticle == null) return;

        if (_gameCamera == null)
            _gameCamera = Camera.main;

        if (_gameCamera == null) return;

        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = -_gameCamera.transform.position.z;

        Vector2 mouseWorld = _gameCamera.ScreenToWorldPoint(mouseScreen);
        Vector2 direction = mouseWorld - (Vector2)transform.position;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        // Convert direction → angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Create rotation (Z-axis for 2D)
        Quaternion rotation = Quaternion.Euler(angle, -90, -90);

        _shotParticle.transform.rotation = rotation;

        _shotParticle.Play();
    }

    void ApplyHoming()
    {
        if (_awaitingLaunch) return;
        if (_brickPool == null) return;

        Vector2 vel = _rigidbody.linearVelocity;
        float speed = vel.magnitude;
        if (speed < 0.01f) return; // not moving

        Vector2 dir = vel.normalized;

        // only apply homing when ball is travelling mostly horizontally (so it helps escape horizontal trap)
        if (Mathf.Abs(dir.y) >= _minVerticalForHoming) return;

        // get nearest active brick from pool
        GameObject target = _brickPool.GetNearestActiveBrick(transform.position, _homingMaxDistance);
        if (target == null) return;

        Vector2 toTarget = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;

        // avoid abrupt U-turns: if the target is almost directly behind, reduce or skip homing
        float forwardDot = Vector2.Dot(dir, toTarget); // 1 = same dir, -1 = opposite
        if (forwardDot < -0.8f) return;

        // compute new direction (lerp between current and target direction)
        float s = _homingStrength;
        Vector2 newDir = Vector2.Lerp(dir, toTarget, s).normalized;
        // preserve speed
        _rigidbody.linearVelocity = newDir * speed;
    }
    public void ResetPosition()
    {
        _spriteRenderer.enabled = false;
        _trailRenderer.enabled = false;
        _circleCollider.enabled = false;
        _abilityManager.NotifyBallDestroyed(this);
        //StartCoroutine(ResettingBall());
    }
    public void PrepareForLaunch(Vector3 position)
    {
        transform.position = position;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _awaitingLaunch = true;
    }

    public void Launch(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
            return;

        direction.Normalize();

        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.linearVelocity = direction * _launchSpeed;
        transform.up = -direction;

        _awaitingLaunch = false;
        _ballDirectionArrow.SetEnableArrow(false);

        AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onBallShoot, transform.position);
    }
    public void DestroyCopyBall()
    {
        _abilityManager.NotifyBallDestroyed(this);
        Destroy(gameObject);

    }
    void ResettingBall()
    {
        //yield return new WaitForSeconds(_respawnTime);
        ResetPosition();
        AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onBallRespawn, transform.position);

        ActivateBall();
        StartCoroutine(AnimateBallRespawn());
        PrepareForLaunch(_respawnPos.position);
        ResetBallRespawnTimer();

    }

    IEnumerator AnimateBallRespawn()
    {
        _reviveParticle.Play();
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = _startingScale * _capscaleMultiplier;

        float time = 0f;

        while (time < animationDuration)
        {
            float normalized = time / animationDuration;
            float curveValue = easeOutElastic.Evaluate(normalized);

            transform.localScale =
                Vector3.LerpUnclamped(startScale, targetScale, curveValue);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = _startingScale;
    }

    void IncreaseCombo()
    {
        _currentCombo++;
        if (_currentCombo >= _feverThreshold)
        {
            _particleTrail.SetActive(true);
        }
    }
    void ResetCombo()
    {
        _currentCombo = 0;
        _particleTrail.SetActive(false);
    }
    public void ResetBallRespawnTimer()
    {
        _isBallDead = true;
        _currentRespawnTimer = _respawnTime; 
    }
    public void DeactivateBall()
    {
        _ballDirectionArrow.SetEnableArrow(false);
        _circleCollider.enabled = false;
        _spriteRenderer.enabled = false;
        _trailRenderer.enabled = false;
    }
    public void ActivateBall()
    {
        _ballDirectionArrow.SetEnableArrow(true);
        _circleCollider.enabled = true;
        _spriteRenderer.enabled = true;
        _trailRenderer.Clear();
        _trailRenderer.enabled = true;
    }
    public void PlayPaddleDeathEffect() => _deathEffect.gameObject.SetActive(true);

    void PlayBallDestroyAudio() => AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onBallDestroy, transform.position);
    void PlayBallRespawnAudio() => AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onBallRespawn, transform.position);

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_awaitingLaunch)
            return;

        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Paddle") || other.gameObject.CompareTag("Brick") || other.gameObject.CompareTag("Shield"))
        {
            GlobalFeedbackManager.Instance.SetSizeCapForBall();
            GlobalFeedbackManager.Instance.PlayGlobalFeedback?.Invoke();
            OnBallHit?.Invoke();

            Vector2 avgNormal = Vector2.zero;
            int contacts = Mathf.Max(1, other.contactCount);
            for (int i = 0; i < other.contactCount; i++)
            {
                avgNormal += other.GetContact(i).normal;
            }
            avgNormal /= contacts;

            if (avgNormal.sqrMagnitude > 0.0001f)
                avgNormal.Normalize();
            else
                avgNormal = Vector2.up; // fallback

            Vector2 opposite = -avgNormal;
            transform.up = opposite;
            if (other.gameObject.GetComponent<BrickBar>() != null)
            {
                if (_copyBall)
                {
                    _currentBounce++;
                    if (_currentBounce > _maxBounce)
                        Destroy(gameObject);
                }
                _currentDelayTime = _delayTimeAfterHit;

                OnBrickHit?.Invoke();
                _abilityManager.NotifyBrickHit(other.gameObject.GetComponent<BrickBar>(), (_baseDamage));
            }
        }
    }
    //HELPER
    public int GetBallBaseDamage() => _baseDamage;
    public void SetTimeScaleSmooth(float target, float duration)
    {
        if (Mathf.Approximately(_currentTargetTimeScale, target))
            return;

        _currentTargetTimeScale = target;

        if (_timeScaleRoutine != null)
            StopCoroutine(_timeScaleRoutine);

        _timeScaleRoutine = StartCoroutine(LerpTimeScale(target, duration));
    }
    IEnumerator LerpTimeScale(float target, float duration)
    {
        float start = Time.timeScale;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float newScale = Mathf.Lerp(start, target, t);

            TimeManager.SetCustomTimeScale(newScale);

            time += Time.unscaledDeltaTime; // IMPORTANT
            yield return null;
        }

        TimeManager.SetCustomTimeScale(target);
    }
    void MinusMana(int val)
    {
        _currentManaAmount -= val;
        if (_currentManaAmount < 0)
            _currentManaAmount = 0;
    }
    public void SetHomingValue(float value) => _homingStrength = value;
    public float GetCurrentManaAmount() => _currentManaAmount;
    public float GetMaxManaAmount() => _maxManaAmount;
    public void IncreaseHomingStrength(float val) => _homingStrength += val;

    public void SetCursorState(bool state)
    {
        Cursor.visible = state;

        if (Cursor.visible)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Confined;

    }

}