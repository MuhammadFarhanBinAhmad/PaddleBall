using UnityEngine;

public interface IExplosionContextModifier
{
    void ModifyExplosionContextAdd(HitContext hitCtx, AbilityContext explosionCtx);
    void ModifyExplosionContextSubtract(HitContext hitCtx, AbilityContext explosionCtx);
    void ModifyExplosionContextMultiply(HitContext hitCtx, AbilityContext explosionCtx);
    void ModifyExplosionContextDivide(HitContext hitCtx, AbilityContext explosionCtx);

}
