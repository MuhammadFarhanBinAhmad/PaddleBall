using UnityEngine;

public class RemnantsAbility : ABSAbility
{
    protected ExplosionPool _explosionPool;
    HitContext _hitContext;

    private void Start()
    {
        _explosionPool = FindAnyObjectByType<ExplosionPool>();
    }
    public override void OnHitResolved(HitContext ctx)
    {
        _hitContext = ctx;
    }
    public override void OnBrickDestroy(BrickBar bar)
    {
        

        AbilityContext ectx = new AbilityContext
        {
            _source = gameObject,
            _position = bar.transform.position,
            _statusType = _SOAbilityEffect._statusType
        };
        ectx._Stats[STATID.BASE_DAMAGE] = _hitContext._damageValue * _SOAbilityEffect._explosionDamageMultiplier;
        ectx._Stats[STATID.SCALE_MULTIPLIER] = _SOAbilityEffect._scaleSizeMultiplier;

        // Let other abilities modify the explosion data
        _abilityManager.ApplyExplosionModifiers(_hitContext, ectx);
    }
}
