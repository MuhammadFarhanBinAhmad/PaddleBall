using UnityEngine;

public class Perfect10 : ABSAbility
{
    int _currentCombo;
    public override void BeforeHitMultiply(HitContext ctx)
    {
        _currentCombo++;
        if (_currentCombo % _SOAbilityEffect._comboThreshold == 0)
        {
            float addDmg = ctx._damageValue * _SOAbilityEffect._baseDamageMultiplier;
            ctx._damageValue += (int)addDmg;
        }


    }
    public override void OnBallDestroy(Ball ball)
    {
        _currentCombo = 0;
    }
}
