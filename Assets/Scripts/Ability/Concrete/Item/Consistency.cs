using System;
using Unity.Mathematics;
using UnityEngine;

public class Consistency : ABSAbility
{

    public int _currentCombo;
    int _baseDamageIncrease;

    public override void ModifyHit(HitContext ctx)
    {
        ctx._damageValue += _baseDamageIncrease;
    }
    public override void OnHit(HitContext ctx)
    {
        _currentCombo++;
        if (_currentCombo >= _SOAbilityEffect._comboThreshold)
        {
            _baseDamageIncrease += (int)_SOAbilityEffect._baseDamagePlus;
            _currentCombo = 0;
        }
    }
    public override void OnBallDestroy(Ball ball)
    {
        _currentCombo = 0;
        _baseDamageIncrease = 0;
    }
}
