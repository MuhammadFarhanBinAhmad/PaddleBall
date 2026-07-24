using Unity.VisualScripting;
using UnityEngine;

public class ExplosiveAbility : ABSAbility
{

    protected ExplosionPool _explosionPool;

    private void Start()
    {
        _explosionPool = FindAnyObjectByType<ExplosionPool>();
    }

    public override void OnHit(HitContext ctx)
    {
        if (_explosionPool == null) return;

        GameObject explosionGO = _explosionPool.GetExplosion();
        explosionGO.transform.position = transform.position;

        Vector3 explosionPos;
        if (ctx._health != null)
        {
            explosionPos = ctx._health.transform.position;
        }
        else
        {
            explosionPos = transform.position;
        }

        var ed = explosionGO.GetComponent<ExplosionDamage>();
        if (ed == null) return;

        ExplosionContext ectx = new ExplosionContext
        {
            _source = gameObject,
            _position = explosionPos,
            _statusEffect = null
        };
        ectx._Stats[STATID.BASE_DAMAGE] = _SOAbilityEffect._abilityBaseDamageValue;
        ectx._Stats[STATID.SCALE_MULTIPLIER] = _SOAbilityEffect._scaleSizeMultiplier;

        // Let other abilities modify the explosion data
        _abilityManager.ApplyExplosionModifiers(ctx, ectx);
        ed.Initialize(ectx,true);
    }
}
