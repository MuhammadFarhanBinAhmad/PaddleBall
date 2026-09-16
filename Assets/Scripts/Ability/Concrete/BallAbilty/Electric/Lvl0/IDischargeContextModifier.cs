using UnityEngine;

public interface IDischargeContextModifier 
{
    void ModifyDischargeAdd(HitContext hitCtx = null, AbilityContext dischargeCtx = null);
    void ModifyDischargeSubtract(HitContext hitCtx = null, AbilityContext dischargeCtx = null);
    void ModifyDischargeMultiple(HitContext hitCtx = null, AbilityContext dischargeCtx = null);
    void ModifyDischargeDivide(HitContext hitCtx = null, AbilityContext dischargeCtx = null);

}
