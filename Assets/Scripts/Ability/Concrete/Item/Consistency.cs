using System;
using Unity.Mathematics;
using UnityEngine;

public class Consistency : ABSAbility
{

    int _currentThreshold;

    public override void ModifyBaseValue(HitContext ctx)
    {
        if (_ball._currentCombo % _SOAbilityEffect._comboThreshold == 0)
        {
            int threshold = _SOAbilityEffect._comboThreshold;
            _currentThreshold = _ball._currentCombo / threshold;

            if (threshold <= 0)
                return;
        }
        int _baseDamageIncrease = _currentThreshold * (int)_SOAbilityEffect._baseDamagePlus;
        ctx._damageValue += _baseDamageIncrease;
    }
    public override void OnBallDestroy(Ball ball)
    {
        _currentThreshold = 0;
    }
}
