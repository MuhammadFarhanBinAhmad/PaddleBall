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

        if (_explosionPool == null) return;

        GameObject explosionGO = _explosionPool.GetExplosion();
        explosionGO.transform.position = transform.position;
        var ed = explosionGO.GetComponent<ExplosionDamage>();
        if (ed == null) return;

        ExplosionContext ectx = new ExplosionContext
        {
            _source = gameObject,
            _position = ctx._health.transform.position,
            _statusEffect = null
        };
        ectx._Stats[STATID.BASE_DAMAGE] = _SOAbilityEffect._abilityBaseDamageValue;
        ectx._Stats[STATID.SCALE_MULTIPLIER] = _SOAbilityEffect._scaleSizeMultiplier;

        // Let other abilities modify the explosion data
        _abilityManager.ApplyExplosionModifiers(_context, ectx);
        ed.Initialize(ectx, true);

    }
}
