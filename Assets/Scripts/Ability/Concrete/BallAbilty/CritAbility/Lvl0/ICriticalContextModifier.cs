using UnityEngine;

public interface ICriticalContextModifier
{
    void ModifyCriticalContextAdd(HitContext hitCtx, AbilityContext critContext);
    void ModifyCriticalContextSubtract(HitContext hitCtx, AbilityContext critContext);
    void ModifyCriticalContextMultiply(HitContext hitCtx, AbilityContext critContext);
    void ModifyCriticalContextDivide(HitContext hitCtx, AbilityContext critContext);

}
