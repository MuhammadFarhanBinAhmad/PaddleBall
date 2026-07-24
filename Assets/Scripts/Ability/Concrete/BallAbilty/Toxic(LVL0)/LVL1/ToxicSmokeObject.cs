using System.Collections;
using UnityEngine;

public class ToxicSmokeObject : ABSAbility
{
    [Header("Lifetime")]
    [SerializeField] private float _timeBeforeDespawn = 3f;
    [SerializeField] private float _shrinkDuration = 0.25f;

    [Header("Damage")]
    [SerializeField] private float _damageTimeInterval = 0.5f;
    [SerializeField] private float _damageRadius = 1.5f;
    [SerializeField] private LayerMask _brickLayer;

    private Vector3 _startScale;

    private Coroutine _despawnRoutine;
    private Coroutine _damageRoutine;

    // Cached collider buffer (no allocations)
    private readonly Collider2D[] _hits = new Collider2D[32];
    private ContactFilter2D _filter;

    // Cached context
    private ToxicContext _cachedContext;

    private void Awake()
    {
        if (_abilityManager == null)
            _abilityManager = FindAnyObjectByType<AbilityManager>();

        _filter = new ContactFilter2D();
        _filter.SetLayerMask(_brickLayer);
        _filter.useTriggers = true;
    }

    private void OnEnable()
    {
        _startScale = transform.localScale;

        _cachedContext = CreateToxicContext();

        if (_despawnRoutine != null)
            StopCoroutine(_despawnRoutine);

        if (_damageRoutine != null)
            StopCoroutine(_damageRoutine);

        _despawnRoutine = StartCoroutine(DespawnAfterDelay());
        _damageRoutine = StartCoroutine(DamageRoutine());
    }

    private void OnDisable()
    {
        if (_despawnRoutine != null)
            StopCoroutine(_despawnRoutine);

        if (_damageRoutine != null)
            StopCoroutine(_damageRoutine);

        transform.localScale = _startScale;
    }

    IEnumerator DamageRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(_damageTimeInterval);

        while (true)
        {
            int count = Physics2D.defaultPhysicsScene.OverlapCircle(
                transform.position,
                _damageRadius,
                _filter,
                _hits
            );

            for (int i = 0; i < count; i++)
            {
                Collider2D col = _hits[i];

                if (!col.TryGetComponent(out BrickBar brick))
                    continue;

                _abilityManager.ApplyToxicModifiers(_cachedContext);
                brick._brickHealthComponent.ApplyStatus(_cachedContext);
            }

            yield return wait;
        }
    }
    IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(_timeBeforeDespawn);

        yield return ShrinkAndDisable();
    }

    IEnumerator ShrinkAndDisable()
    {
        float t = 0f;

        while (t < _shrinkDuration)
        {
            float percent = t / _shrinkDuration;

            transform.localScale =
                Vector3.Lerp(_startScale, Vector3.zero, percent);

            t += Time.deltaTime;

            yield return null;
        }

        transform.localScale = Vector3.zero;

        gameObject.SetActive(false);
    }

    ToxicContext CreateToxicContext()
    {
        ToxicContext ctx = new ToxicContext
        {
            _abililty = this,
            _statusType = _SOAbilityEffect._statusType
        };

        ctx._Stats[STATID.STACKS_TO_ADD] = _SOAbilityEffect._stacksToAdd;
        ctx._Stats[STATID.MAX_STACKS] = _SOAbilityEffect._maxStacks;
        ctx._Stats[STATID.DAMAGE_PER_STACK] = _SOAbilityEffect._damagePerStack;
        ctx._Stats[STATID.STACK_LIFETIME] = _SOAbilityEffect._stackLifeTime;
        ctx._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] = _SOAbilityEffect._timeBeforeEffectActivate;
        ctx._Stats[STATID.SPEED_MULTIPLIER] = _SOAbilityEffect._speedMultiplier;

        ctx._Statsbool[STATID.RESET_STACK_TIMER] = _SOAbilityEffect._resetStackTimer;
        ctx._Statsbool[STATID.AFFECTS_SPEED] = _SOAbilityEffect._affectSpeed;

        return ctx;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _damageRadius);
    }
#endif
}