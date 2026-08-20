using Unity.VisualScripting;
using UnityEngine;

public class FirstStrike : ABSAbility
{
    bool _firstStike = true;
    public override void OnHitMultiply(HitContext ctx)
    {
        if(_firstStike)
        {
            ctx._damageValue = (int)(_SOAbilityEffect._baseDamageMultiplier * (float)ctx._damageValue);
            _firstStike = false;
        }
    }
    public override void OnBallDestroy(Ball ball)
    {
        _firstStike = true;
    }
}
