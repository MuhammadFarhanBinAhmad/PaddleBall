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
        statusCtx._Stats[STATID.STACKS_TO_ADD] = _SOAbilityEffect._stacksToAdd;
        statusCtx._Stats[STATID.MAX_STACKS] = _SOAbilityEffect._maxStacks;
        statusCtx._Stats[STATID.DAMAGE_PER_STACK] = _SOAbilityEffect._damagePerStack;
        statusCtx._Stats[STATID.STACK_LIFETIME] = _SOAbilityEffect._stackLifeTime;
        statusCtx._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] = _SOAbilityEffect._timeBeforeEffectActivate;
        statusCtx._Statsbool[STATID.RESET_STACK_TIMER] = _SOAbilityEffect._resetStackTimer;
        statusCtx._Statsbool[STATID.AFFECTS_SPEED] = _SOAbilityEffect._affectSpeed;
        statusCtx._Stats[STATID.SPEED_MULTIPLIER] = _SOAbilityEffect._speedMultiplier;
        _abilityManager.ApplyDischargeModifiers(ctx, statusCtx);

        ctx._health.ApplyStatus(
            statusCtx
        );
        ctx._health.SpawnStatusVFX(STATUSTYPE.DISCHARGE, _vfxBuildPrefab, _vfxPopPrefab);
    }
}
