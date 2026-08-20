using UnityEngine;

public interface IExplosionContextModifier
{
    void ModifyExplosionContextAdd(HitContext hitCtx, ExplosionContext explosionCtx);
    void ModifyExplosionContextSubtract(HitContext hitCtx, ExplosionContext explosionCtx);
    void ModifyExplosionContextMultiply(HitContext hitCtx, ExplosionContext explosionCtx);
    void ModifyExplosionContextDivide(HitContext hitCtx, ExplosionContext explosionCtx);

}
