using System.Collections;
using UnityEngine;

public class ToxicSmokeObject : ABSAbility
{
    [Header("Lifetime")]
    [SerializeField] private float _timeBeforeDespawn;
    [SerializeField] private float _shrinkDuration;

    [Header("Damage")]
    [SerializeField] private float _damageRadius;
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

    public void Initialize()
    {
        transform.localScale = Vector3.one;
        _cachedContext = CreateToxicContext();
        _abilityManager.ApplyToxicModifiers(_cachedContext);

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
        WaitForSeconds wait = new WaitForSeconds(_cachedContext._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE]);

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

                brick._brickHealthComponent.OnDamage((int)_cachedContext._Stats[STATID.DAMAGE_PER_STACK]);
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

        ctx._Stats[STATID.DAMAGE_PER_STACK] = _SOAbilityEffect._damagePerStack;
        ctx._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] = _SOAbilityEffect._timeBeforeEffectActivate;

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