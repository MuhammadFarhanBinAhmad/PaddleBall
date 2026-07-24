using UnityEngine;

public class ExplosiveDischarge : ABSAbility
{
    protected ExplosionPool _explosionPool;
    HitContext _context;
    private void Start()
    {
        _explosionPool = FindAnyObjectByType<ExplosionPool>();
    }
    public override void OnHitResolved(HitContext ctx)
    {

        var statusCtx = new AbilityContext
        {
            _abililty = this,
            _statusType = _SOAbilityEffect._statusType,
        };
        statusCtx._Stats[STATID.STACKS_TO_ADD] = _SOAbilityEffect._stacksToAdd;
        statusCtx._Stats[STATID.MAX_STACKS] = _SOAbilityEffect._maxStacks;
        statusCtx._Stats[STATID.STACK_LIFETIME] = _SOAbilityEffect._stackLifeTime;
        statusCtx._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] = _SOAbilityEffect._timeBeforeEffectActivate;
        ctx._health.ApplyStatus(
            statusCtx
        );
        _context = ctx;

    }

    public override void ActivateAbility(GameObject brick = null)
    {
        if (_explosionPool == null) return;
        GameObject explosionGO = _explosionPool.GetExplosion();
        explosionGO.transform.position = transform.position;
        var ed = explosionGO.GetComponent<ExplosionDamage>();
        if (ed == null) return;

        ExplosionContext ectx = new ExplosionContext
        {
            _source = gameObject,
            _position = brick.transform.position,
            _statusEffect = null
        };
        ectx._Stats[STATID.BASE_DAMAGE] = _SOAbilityEffect._abilityBaseDamageValue;
        ectx._Stats[STATID.SCALE_MULTIPLIER] = _SOAbilityEffect._scaleSizeMultiplier;

        // Let other abilities modify the explosion data
        _abilityManager.ApplyExplosionModifiers(_context, ectx);
        ed.Initialize(ectx, true);
    }
}
