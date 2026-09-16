using System.Collections.Generic;
using UnityEngine;

public class ConductiveAbility : ABSAbility
{
    public GameObject _vfxBuildPrefab, _vfxPopPrefab;
    public SOAbilityEffect _effect;
    public override void OnHitResolved(HitContext ctx)
    {
        bool isConductive = RNGService.RollCrit(_SOAbilityEffect._baseChance, _SOAbilityEffect._bonusPerFail);

        if (isConductive)
        {
            ctx._health.FindNearbyBricks();
            List<BrickHealthComponent> brickHealthComponents = new List<BrickHealthComponent>();
            brickHealthComponents = ctx._health.GetAllNearbyBrick();
            for (int i = 0; i < brickHealthComponents.Count; i++)
            {
                var statusCtx = new AbilityContext
                {
                    _abililty = this,
                    _statusType = _SOAbilityEffect._statusType,
                };
                statusCtx._Stats[STATID.STACKS_TO_ADD] = _effect._stacksToAdd;
                statusCtx._Stats[STATID.MAX_STACKS] = _effect._maxStacks;
                statusCtx._Stats[STATID.DAMAGE_PER_STACK] = _effect._damagePerStack;
                statusCtx._Stats[STATID.STACK_LIFETIME] = _effect._stackLifeTime;
                statusCtx._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] = _effect._timeBeforeEffectActivate;
                statusCtx._Statsbool[STATID.RESET_STACK_TIMER] = _effect._resetStackTimer;
                _abilityManager.ApplyDischargeModifiers(ctx, statusCtx);

                brickHealthComponents[i].ApplyStatus(
                statusCtx,
                STATUSTYPE.DISCHARGE);
                brickHealthComponents[i].SpawnStatusVFX(STATUSTYPE.DISCHARGE);
            }
        }
    }
}
