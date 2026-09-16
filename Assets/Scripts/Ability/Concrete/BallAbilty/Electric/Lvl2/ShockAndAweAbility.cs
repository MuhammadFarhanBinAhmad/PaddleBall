using UnityEngine;

public class ShockAndAweAbility : ABSAbility , IDischargeContextModifier
{
    public void ModifyDischargeAdd(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        dischargeCtx._Statsbool[STATID.AFFECTS_SPEED] = _SOAbilityEffect._affectSpeed;
        dischargeCtx._Stats[STATID.SPEED_MULTIPLIER] = _SOAbilityEffect._speedMultiplier;
    }

    public void ModifyDischargeDivide(HitContext hitCtx, AbilityContext dischargeCtx)
    {
    }

    public void ModifyDischargeMultiple(HitContext hitCtx, AbilityContext dischargeCtx)
    {
    }

    public void ModifyDischargeSubtract(HitContext hitCtx, AbilityContext dischargeCtx)
    {
    }
}
