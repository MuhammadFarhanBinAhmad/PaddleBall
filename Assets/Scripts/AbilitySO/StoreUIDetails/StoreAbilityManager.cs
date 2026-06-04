using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ITEMRARITY
{
    COMMON,
    RARE,
    LEGENDARY,
    ODDITIES
}

[System.Serializable]
public class ShopLevelState
{
    public int _level;
    public int _purchases;
    public int _baseCost;
    public float _multiplier = 1.25f;

    public int GetPrice()
    {
        return Mathf.RoundToInt(_baseCost * Mathf.Pow(_multiplier, _purchases));
    }
    public void RegisterPurchase() => _purchases++;
}

public class StoreAbilityManager : MonoBehaviour
{
    private TowerManager _towerManager;

    [Header("Ball Ability Database")]
    [SerializeField] private List<SOStoreAbilityContent> explosiveAbilities = new List<SOStoreAbilityContent>();
    [SerializeField] private List<SOStoreAbilityContent> criticalAbilities = new List<SOStoreAbilityContent>();
    [SerializeField] private List<SOStoreAbilityContent> dischargeAbilities = new List<SOStoreAbilityContent>();
    [SerializeField] private List<SOStoreAbilityContent> toxicAbilities = new List<SOStoreAbilityContent>();

    [SerializeField] private ShopLevelState[] _abilityLevelAndCostState = new ShopLevelState[4];

    public List<string> unlockedAbilities = new List<string>();
    private Dictionary<string, SOStoreAbilityContent> abilityLookup;

    [Header("Item Ability Database")]
    //2 list
    [SerializeField] List<SOItemAbilityContentUI> _itemAvailableList = new List<SOItemAbilityContentUI>();
    [SerializeField] List<SOItemAbilityContentUI> _itemPurchaseList = new List<SOItemAbilityContentUI>();
    [SerializeField] List<ItemAbilityButtonUI> _itemButtonsList = new List<ItemAbilityButtonUI>();
    [SerializeField] int[] _itemCostByLevel = new int[4];

    [Header("Store Open")]
    public Action OnStoreOpen, OnStoreClose;
    public bool _storeIsOpen;

    public event Action<string> OnAbilityPurchased;

    [SerializeField] int _maxReroll;
    int _numberOfReroll;


    private void Awake()
    {
        _towerManager = FindAnyObjectByType<TowerManager>();

    }
    private void Start()
    {
        ResetRoroll();
        SetItemAvailableToPurchase();
        SetUpAbility();
    }
    internal void SetUpAbility(STATUSTYPE _type = STATUSTYPE.EXPLOSION)
    {
        for (int i = 0; i < _abilityLevelAndCostState.Length; i++)
        {
            if (_abilityLevelAndCostState[i] == null)
            {
                _abilityLevelAndCostState[i] = new ShopLevelState();
            }

            _abilityLevelAndCostState[i]._level = i;
        }
        switch (_type)
        {
            case STATUSTYPE.EXPLOSION:
                {
                    abilityLookup = explosiveAbilities
                    .Where(a => a != null && !string.IsNullOrEmpty(a.abilityID))
                    .ToDictionary(a => a.abilityID, a => a);
                    break;
                }
            case STATUSTYPE.CRIT:
                {
                    abilityLookup = criticalAbilities
                    .Where(a => a != null && !string.IsNullOrEmpty(a.abilityID))
                    .ToDictionary(a => a.abilityID, a => a);
                    break;
                }
            case STATUSTYPE.TOXIC:
                {
                    abilityLookup = toxicAbilities
                    .Where(a => a != null && !string.IsNullOrEmpty(a.abilityID))
                    .ToDictionary(a => a.abilityID, a => a);
                    break;
                }
            case STATUSTYPE.DISCHARGE:
                {
                    abilityLookup = dischargeAbilities
                    .Where(a => a != null && !string.IsNullOrEmpty(a.abilityID))
                    .ToDictionary(a => a.abilityID, a => a);
                    break;
                }

        }



    }
    public bool CanPurchase(string abilityID)
    {
        if (!abilityLookup.ContainsKey(abilityID))
            return false;

        if (unlockedAbilities.Contains(abilityID))
            return false;

        SOStoreAbilityContent ability = abilityLookup[abilityID];
        int abilityLevel = ability.ability_Level;

        if (abilityLevel < 0 || abilityLevel >= _abilityLevelAndCostState.Length)
            return false;

        int price = _abilityLevelAndCostState[abilityLevel].GetPrice();

        if (_towerManager.GetTotalPureEssenceCount() < price)
            return false;

        if (!ability._availableToPurchaseAtStart)
        {
            foreach (string requiredID in ability.requiredAbilityIDs)
            {
                if (!unlockedAbilities.Contains(requiredID))
                    return false;
            }
        }

        return true;
    }

    public bool PurchaseAbility(string abilityID)
    {
        if (!CanPurchase(abilityID))
            return false;

        SOStoreAbilityContent ability = abilityLookup[abilityID];
        int abilityLevel = ability.ability_Level;

        int price = _abilityLevelAndCostState[abilityLevel].GetPrice();

        _towerManager.DeductPureEssence(price);
        unlockedAbilities.Add(abilityID);

        _abilityLevelAndCostState[abilityLevel].RegisterPurchase();

        OnAbilityPurchased?.Invoke(abilityID);

        return true;
    }

    public bool IsUnlocked(string abilityID)
    {
        return unlockedAbilities.Contains(abilityID);
    }

    public bool IsAvailableToPurchase(string abilityID)
    {
        if (!abilityLookup.ContainsKey(abilityID))
            return false;

        SOStoreAbilityContent ability = abilityLookup[abilityID];

        if (ability._availableToPurchaseAtStart)
            return true;

        foreach (string requiredID in ability.requiredAbilityIDs)
        {
            if (!unlockedAbilities.Contains(requiredID))
                return false;
        }

        return true;
    }

    public List<SOStoreAbilityContent> GetAbilityList(STATUSTYPE _type)
    {
        List<SOStoreAbilityContent> list = null;

        switch (_type)
        {
            case STATUSTYPE.EXPLOSION:
                {
                    list = new List<SOStoreAbilityContent>(explosiveAbilities);
                    break;
                }
            case STATUSTYPE.DISCHARGE:
                {
                    list = new List<SOStoreAbilityContent>(dischargeAbilities);
                    break;
                }
            case STATUSTYPE.CRIT:
                {
                    list = new List<SOStoreAbilityContent>(criticalAbilities);
                    break;
                }
            case STATUSTYPE.TOXIC:
                {
                    list = new List<SOStoreAbilityContent>(toxicAbilities);
                    break;
                }
        }
        return list;

    }

    public int GetAbilityCost(int level)
    {
        if (level < 0 || level >= _abilityLevelAndCostState.Length || _abilityLevelAndCostState[level] == null)
            return 0;

        return _abilityLevelAndCostState[level].GetPrice();
    }

    public void SetItemAvailableToPurchase()
    {
        // Safety check
        if (_itemAvailableList.Count == 0 || _itemButtonsList.Count == 0)
            return;

        // Create a shuffled copy of available items
        List<SOItemAbilityContentUI> shuffled = new List<SOItemAbilityContentUI>(_itemAvailableList);

        for (int i = 0; i < shuffled.Count; i++)
        {
            int rand = UnityEngine.Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[rand]) = (shuffled[rand], shuffled[i]);
        }

        // Assign to buttons
        for (int i = 0; i < _itemButtonsList.Count; i++)
        {
            if (i >= shuffled.Count)
                break;

            _itemButtonsList[i].SetItemAbilityContent(shuffled[i]);
        }
    }
    public int GetItemCost(ITEMRARITY rarity)
    {
        int cost = 0;
        switch (rarity)
        {
            case ITEMRARITY.COMMON:
                cost = _itemCostByLevel[0];
                break;
            case ITEMRARITY.RARE:
                cost = _itemCostByLevel[1];
                break;
            case ITEMRARITY.LEGENDARY:
                cost = _itemCostByLevel[2];
                break;
            case ITEMRARITY.ODDITIES:
                cost = _itemCostByLevel[3];
                break;
        }
        return cost;
    }

    public void RerollItem()
    {
        if(_numberOfReroll > 0)
        {
            SetItemAvailableToPurchase();
            _numberOfReroll--;
        }
    }
    public void ResetRoroll() => _numberOfReroll = _maxReroll;
}