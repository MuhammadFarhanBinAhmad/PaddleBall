using System;
using UnityEngine;

[Flags]
public enum AOEMODIFIER
{
    NONE = 0,
    REGEN = 1 << 0,
    SHIELD = 1 << 1,
    SPEED = 1 << 2
}

[CreateAssetMenu(fileName = "Brick Modifier Effect", menuName = "Brick/Brick Modifier Effect")]

public class SOBrickModifier : ScriptableObject
{
    public ITEMRARITY _itemRarity;
    public int _modifierCost, _dayToUnlock;
    public bool _healthModifier, _shieldModifier, _speedModifier, _aoeModifier;

    public BrickModifierBase _modifierPrefab;

    [GroupUnder(nameof(_healthModifier))]
    public float _healthAddOn;
    [GroupUnder(nameof(_healthModifier))]
    public float _healthMultiplier;
    [GroupUnder(nameof(_healthModifier))]
    public float _regenRate;
    [GroupUnder(nameof(_healthModifier))]
    public int _regenValue;
    [GroupUnder(nameof(_shieldModifier))]
    public int _shieldValue;
    [GroupUnder(nameof(_speedModifier))]
    public float _speedAdd, _speedMultiplier;
    [GroupUnder(nameof(_aoeModifier))]
    public AOEMODIFIER _modifier;
    [GroupUnder(nameof(_aoeModifier))]
    public float _aoeRadius;
}
