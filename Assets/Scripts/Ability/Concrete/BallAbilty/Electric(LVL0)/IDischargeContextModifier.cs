using UnityEngine;

public interface IDischargeContextModifier 
{
    void ModifyDischargeAdd(HitContext hitCtx, AbilityContext dischargeCtx);
    void ModifyDischargeSubtract(HitContext hitCtx, AbilityContext dischargeCtx);
    void ModifyDischargeMultiple(HitContext hitCtx, AbilityContext dischargeCtx);
    void ModifyDischargeDivide(HitContext hitCtx, AbilityContext dischargeCtx);

}
