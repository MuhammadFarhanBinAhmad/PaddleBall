using Unity.VisualScripting;
using UnityEngine;

public class FirstStrike : ABSAbility
{
    bool _firstStike = true;
    public override void OnHit(HitContext ctx)
    {
        if(_firstStike)
        {
            float dmg = (float)ctx._damageValue;
            ctx._damageValue = (int)(_SOAbilityEffect._baseDamageMultiplier * dmg);
            _firstStike = false;
        }
    }
    public override void OnBallDestroy(Ball ball)
    {
        _firstStike = true;
    }
}
