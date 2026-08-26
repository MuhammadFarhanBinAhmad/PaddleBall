using UnityEngine;


[CreateAssetMenu(menuName = "Ability/Ability Effect")]
public class SOAbilityEffect : ScriptableObject
{
    public string _abilityName;

    public STATUSTYPE _statusType;

    [Header("Runtime")]
    public GameObject _abilityPrefab;

    public bool _genericEffect;
    public bool _applyStatus;
    public bool _spawnEffect;
    public bool _critEffect;
    public bool _explosionEffect;
    public bool _counterEffect;
    public bool _shieldEffect;

    //-----------------Generic-----------------//
    [GroupUnder(nameof(_genericEffect))]
    public int _abilityBaseDamageValue;
    [GroupUnder(nameof(_genericEffect))]
    public int _baseDamagePlus;
    [GroupUnder(nameof(_genericEffect))]
    public int _baseDamageMinus;
    [GroupUnder(nameof(_genericEffect))]
    public float _baseDamageMultiplier;//Value of abiltity effect to change. Be use to replace, add,minus,etc. Is multiplier(eg.thershold, base damage, etc.)
    [GroupUnder(nameof(_genericEffect))]
    public float _speedMultiplier;
    [GroupUnder(nameof(_genericEffect))]
    public float _bonusPerFail;
    [GroupUnder(nameof(_genericEffect))]
    public float _scaleSizeMultiplier;
    [GroupUnder(nameof(_genericEffect))]
    public float _timer;
    [GroupUnder(nameof(_genericEffect))]
    public int _threshold;
    //-----------------Counter-----------------//
    [GroupUnder(nameof(_counterEffect))]
    public float _timeRate;
    [GroupUnder(nameof(_counterEffect))]
    public int _comboThreshold;
    //-----------------Toxic/Stacking-----------------//
    [GroupUnder(nameof(_applyStatus))]
    public int _maxStacks;
    [GroupUnder(nameof(_applyStatus))]
    public int _stacksToAdd;
    [GroupUnder(nameof(_applyStatus))]
    public int _maxStacksToAdd;
    [GroupUnder(nameof(_applyStatus))]
    public int _increaseStacksToAdd;
    [GroupUnder(nameof(_applyStatus))]
    public int _damagePerStack;
    [GroupUnder(nameof(_applyStatus))]
    public float _stackLifeTime;
    [GroupUnder(nameof(_applyStatus))]
    public float _timeBeforeEffectActivate;
    [GroupUnder(nameof(_applyStatus))]
    public float _modifyTimeBeforeEffectActivate;
    [GroupUnder(nameof(_applyStatus))]
    public bool _resetStackTimer;
    [GroupUnder(nameof(_applyStatus))]
    public bool _affectSpeed;
    //-----------------Spawn-----------------//
    [GroupUnder(nameof(_spawnEffect))]
    public int _amountToSpawn;
    [GroupUnder(nameof(_spawnEffect))]
    public GameObject _itemToSpawn;
    //-----------------Crit-----------------//
    [GroupUnder(nameof(_critEffect))]
    public float _baseCritChance;
    [GroupUnder(nameof(_critEffect))]
    public float _critMultiplier;
    [GroupUnder(nameof(_critEffect))]
    public float _modiftCritMultiplier;
    [GroupUnder(nameof(_critEffect))]
    public int _layerToDestroy;
    //-----------------Explosive-----------------//
    [GroupUnder(nameof(_explosionEffect))]
    public float _explosionDamageMultiplier;
    [GroupUnder(nameof(_explosionEffect))]
    public float _explosionSizeMultiplier;
    //-----------------Shield-----------------//
    [GroupUnder(nameof(_shieldEffect))]
    public float _shieldMultiplier;
    [GroupUnder(nameof(_shieldEffect))]
    public int _shieldAdd;
    [GroupUnder(nameof(_shieldEffect))]
    public float _shieldModifyShieldRegenRate;
    [GroupUnder(nameof(_shieldEffect))]
    public float _shieldModifyCooldown;

}
