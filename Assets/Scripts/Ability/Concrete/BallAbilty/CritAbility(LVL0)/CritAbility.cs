using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Jobs;

public class CritAbility : ABSAbility
{

    public override void OnHitMultiply(HitContext ctx)
    {
        AbilityContext cctx = new AbilityContext { };
        cctx._Stats[STATID.BASE_DAMAGE] = ctx._damageValue;
        cctx._Stats[STATID.CRIT_CHANCE] = _SOAbilityEffect._baseChance;
        cctx._Stats[STATID.CRIT_MULTIPLIER] = _SOAbilityEffect._critMultiplier + +ctx._modifyChance;


        bool isCrit =  RNGService.RollCrit(cctx._Stats[STATID.CRIT_CHANCE], _SOAbilityEffect._bonusPerFail);
        
        if (isCrit)
            ctx._status = STATUSTYPE.CRIT;

        _abilityManager.ApplyCriticalModifiers(ctx, cctx);

        if (isCrit)
            ctx._damageValue = Mathf.CeilToInt(cctx._Stats[STATID.BASE_DAMAGE] * cctx._Stats[STATID.CRIT_MULTIPLIER]
            );
        else
            ctx._damageValue = (int)cctx._Stats[STATID.BASE_DAMAGE];


        return; // only one crit owner

    }
}
