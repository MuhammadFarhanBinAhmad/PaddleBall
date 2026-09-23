using UnityEngine;


[CreateAssetMenu(menuName = "Ability/Ability Effect")]
public class SOAbilityEffect : ScriptableObject
{


    public string _abilityName;

    public STATUSTYPE _statusType;
    public TYPE _type;

    [Header("Runtime")]
    public GameObject _abilityPrefab;

    [SerializeField] bool _genericEffect;
    [SerializeField] bool _applyStatus;
    [SerializeField] bool _spawnEffect;
    [SerializeField] bool _chanceEffect;
    [SerializeField] bool _explosionEffect;
    [SerializeField] bool _counterEffect;
    [SerializeField] bool _shieldEffect;

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
    [GroupUnder(nameof(_genericEffect))]
    public float _essenceValueAdd;
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
    public float _damagePerStackMultiplier;
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
    //-----------------Crit/Chance-----------------//
    [GroupUnder(nameof(_chanceEffect))]
    public float _baseChance;
    [GroupUnder(nameof(_chanceEffect))]
    public float _critMultiplier;
    [GroupUnder(nameof(_chanceEffect))]
    public float _modifyCritMultiplier;
    [GroupUnder(nameof(_chanceEffect))]
    public int _layerToDestroy;
    [GroupUnder(nameof(_chanceEffect))]
    public int _instantKillThreshold;
    //-----------------Explosive-----------------//
    [GroupUnder(nameof(_explosionEffect))]
    public float _explosionDamageMultiplier;
    [GroupUnder(nameof(_explosionEffect))]
    public float _explosionRadius ;
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
