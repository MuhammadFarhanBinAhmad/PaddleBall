using UnityEngine;

public class Veteran : ABSAbility
{
    int _brickDestroyed;
    int _stack;

    public override void ModifyBaseValue(HitContext ctx)
    {
        ctx._damageValue += _stack * _SOAbilityEffect._baseDamagePlus;
    }
    public override void OnBrickDestroy(BrickBar bar)
    {
        _brickDestroyed++;
        
        if (_brickDestroyed % _SOAbilityEffect._threshold == 0)
        {
            _brickDestroyed = 0;
            _stack++;
        }
    }
}
