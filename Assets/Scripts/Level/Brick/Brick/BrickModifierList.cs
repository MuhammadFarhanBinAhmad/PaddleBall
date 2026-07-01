using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrickModifierList : MonoBehaviour
{
    [SerializeField]TimeManager _timeManager;

    [Header("BrickModifierList")]
    public List<SOBrickModifier> _brickCommonModifier = new List<SOBrickModifier>();
    public List<SOBrickModifier> _brickRareModifier = new List<SOBrickModifier>();
    public List<SOBrickModifier> _brickLegendaryModifier = new List<SOBrickModifier>();
    public List<SOBrickModifier> _brickAvailableCommonModifierToSpawn = new List<SOBrickModifier>();
    public List<SOBrickModifier> _brickAvailableRareModifierToSpawn = new List<SOBrickModifier>();
    public List<SOBrickModifier> _brickAvailableLegendaryModifierToSpawn = new List<SOBrickModifier>();

    [Header("BrickModifierProbability")]
    public TWEENTYPE _modifierTweenType = TWEENTYPE.LINEAR;
    [SerializeField] float _firstModiferCheckMinValue, _firstModiferCheckMaxValue;
    [SerializeField] int _totalFirstModiferCheckValue;
    public float[] _firstModiferCheck;
    public int _dayFirstModiferCheckUnlock;
    [SerializeField] float _secondModiferCheckMinValue, _secondModiferCheckMaxValue;
    [SerializeField] int _totalSecondModiferCheckValue;
    public float[] _secondModiferCheck;
    public int _daySecondModiferCheckUnlock;

    [Header("BrickRollChances")]
    [Range(0, 100)] public float _commonChance;
    [Range(0, 100)] public float _rareChance;
    [Range(0, 100)] public float _legendaryChance;

    private void Awake()
    {
        _timeManager = FindAnyObjectByType<TimeManager>();
    }

    public void CheckModifierToAdd()
    {
        int day = _timeManager.GetTotalDayPass();
        for (int i = 0; i < _brickCommonModifier.Count; i++)
        {
            if (_brickAvailableCommonModifierToSpawn.Contains(_brickCommonModifier[i]))
                continue;

            if (_brickCommonModifier[i]._dayToUnlock == day)
            {
                _brickAvailableCommonModifierToSpawn.Add(_brickCommonModifier[i]);
            }
        }
        for (int i = 0; i < _brickRareModifier.Count; i++)
        {
            if (_brickAvailableRareModifierToSpawn.Contains(_brickRareModifier[i]))
                continue;

            if (_brickRareModifier[i]._dayToUnlock == day)
            {
                _brickAvailableRareModifierToSpawn.Add(_brickRareModifier[i]);
            }
        }
        for (int i = 0; i < _brickLegendaryModifier.Count; i++)
        {
            if (_brickAvailableLegendaryModifierToSpawn.Contains(_brickLegendaryModifier[i]))
                continue;

            if (_brickLegendaryModifier[i]._dayToUnlock == day)
            {
                _brickAvailableLegendaryModifierToSpawn.Add(_brickLegendaryModifier[i]);
            }
        }

    }
    public void TryAddRandomModifier(BrickBar bb, ITEMRARITY _rarity)
    {

        List<SOBrickModifier> candidates = new List<SOBrickModifier>();
        SOBrickModifier chosen = null;
        switch (_rarity)
        {
            case ITEMRARITY.COMMON:
                {
                    int randindex = UnityEngine.Random.Range(0, _brickAvailableCommonModifierToSpawn.Count - 1);
                    chosen = _brickAvailableCommonModifierToSpawn[randindex];
                    break;
                }
            case ITEMRARITY.RARE:
                {
                    int randindex = UnityEngine.Random.Range(0, _brickAvailableRareModifierToSpawn.Count - 1);
                    chosen = _brickAvailableRareModifierToSpawn[randindex];
                    break;
                }
            case ITEMRARITY.LEGENDARY:
                {
                    int randindex = UnityEngine.Random.Range(0, _brickAvailableLegendaryModifierToSpawn.Count - 1);
                    chosen = _brickAvailableLegendaryModifierToSpawn[randindex];
                    break;
                }
        }
        bb.AddModifier(chosen);
    }
    public ITEMRARITY RollRarity()
    {
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll < _commonChance)
        {
            return ITEMRARITY.COMMON;
        }

        if (roll < _commonChance + _rareChance)
        {
            return ITEMRARITY.RARE;
        }

        return ITEMRARITY.LEGENDARY;
    }

    internal void PopulateModifierChanceTable()
    {
        _firstModiferCheck = new float[_totalFirstModiferCheckValue];
        _secondModiferCheck = new float[_totalSecondModiferCheckValue];

        // if only one sample, use start value
        if (_totalFirstModiferCheckValue == 1)
        {
            _firstModiferCheck[0] = _firstModiferCheckMinValue;
            return;
        }
        if (_totalSecondModiferCheckValue == 1)
        {
            _secondModiferCheck[0] = _secondModiferCheckMinValue;
            return;
        }
        int steps = _totalFirstModiferCheckValue - 1; // denom so last element = end value
        for (int i = 0; i < _totalFirstModiferCheckValue; i++)
        {
            float t = (float)i / (float)steps;               // normalized [0,1]
            float eased = TweenService.GetEased(t, _modifierTweenType);      // apply chosen easing
            float val = Mathf.Lerp(_firstModiferCheckMinValue, _firstModiferCheckMaxValue, eased);
            _firstModiferCheck[i] = Mathf.Round(val * 100f) / 100f;
        }
        steps = _totalSecondModiferCheckValue - 1; // denom so last element = end value
        for (int i = 0; i < _totalSecondModiferCheckValue; i++)
        {
            float t = (float)i / (float)steps;               // normalized [0,1]
            float eased = TweenService.GetEased(t, _modifierTweenType);      // apply chosen easing
            float val = Mathf.Lerp(_secondModiferCheckMinValue, _secondModiferCheckMaxValue, eased);
            _secondModiferCheck[i] = Mathf.Round(val * 100f) / 100f;
        }
    }
    internal bool RollForModifier(bool _isFirstModifier)
    {
        float randvalue = UnityEngine.Random.value;

        if (_isFirstModifier)
        {
            int _firstModiferLength = _firstModiferCheck.Count();
            if (randvalue >= _firstModiferCheck[0])
                return true;
            else
                return false;
        }
        else
        {
            int _secondModiferLength = _secondModiferCheck.Count();
            if (randvalue >= _secondModiferCheck[0])
                return true;
            else
                return false;
        }
    }
#if UNITY_EDITOR
    internal void OnValidate()
    {
        // ensure array is updated in editor without entering playmode
        PopulateModifierChanceTable();
    }
#endif
}
