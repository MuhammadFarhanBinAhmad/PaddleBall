using UnityEngine;

public class ElectricAbility : ABSAbility
{
    public GameObject _vfxBuildPrefab, _vfxPopPrefab;
    public override void OnHit(HitContext ctx)
    {
        var statusCtx = new AbilityContext
        {
            _abililty = this,
            _statusType = _SOAbilityEffect._statusType,
        };
        float dmg = ctx._damageValue * _SOAbilityEffect._damagePerStackMultiplier;
        statusCtx._Stats[STATID.STACKS_TO_ADD] = _SOAbilityEffect._stacksToAdd;
        statusCtx._Stats[STATID.MAX_STACKS] = _SOAbilityEffect._maxStacks;
        statusCtx._Stats[STATID.DAMAGE_PER_STACK] = dmg;
        statusCtx._Stats[STATID.STACK_LIFETIME] = _SOAbilityEffect._stackLifeTime;
        statusCtx._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] = _SOAbilityEffect._timeBeforeEffectActivate;
        statusCtx._Statsbool[STATID.RESET_STACK_TIMER] = _SOAbilityEffect._resetStackTimer;
        statusCtx._Statsbool[STATID.AFFECTS_SPEED] = _SOAbilityEffect._affectSpeed;
        statusCtx._Stats[STATID.SPEED_MULTIPLIER] = _SOAbilityEffect._speedMultiplier;
        _abilityManager.ApplyDischargeModifiers(ctx, statusCtx);
        _abilityManager.ApplyStackableModifiers(statusCtx);
        ctx._health.ApplyStatus(
            statusCtx,
            STATUSTYPE.DISCHARGE
        );
        ctx._health.SpawnStatusVFX(STATUSTYPE.DISCHARGE);
    }
}
