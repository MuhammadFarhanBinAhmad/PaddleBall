using UnityEngine;

public class Perfect10 : ABSAbility
{
    int _currentCombo;
    public override void ModifyHit(HitContext ctx)
    {
        _currentCombo++;
        if (_currentCombo % _SOAbilityEffect._comboThreshold == 0)
            ctx._damageValue = (int)(_SOAbilityEffect._baseDamageMultiplier * (float)ctx._damageValue);

    }
    public override void OnBallDestroy(Ball ball)
    {
        _currentCombo = 0;
    }
}
