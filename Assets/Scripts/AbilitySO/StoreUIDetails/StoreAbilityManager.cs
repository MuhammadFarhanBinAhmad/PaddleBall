using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ITEMRARITY
{
    COMMON,
    UNCOMMON,
    RARE,
    LEGENDARY,
    ODDITIES,
    NULL
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
    TowerManager _towerManager;
    TowerUIManager _towerUIManager;

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
    [SerializeField] SOItemAbilityContentUI _emptyItem;
    [SerializeField] List<SOItemAbilityContentUI> _itemPurchasedList = new List<SOItemAbilityContentUI>();
    [SerializeField] List<ItemAbilityButtonUI> _itemButtonsList = new List<ItemAbilityButtonUI>();
    [SerializeField] int[] _itemCostByLevel = new int[4];
    [Header("Store Open")]
    public Action OnStoreOpen, OnStoreClose;
    public bool _storeIsOpen;

    [Header("Item Rarity Weights")]
    [SerializeField, Min(0)] private int _commonWeight;
    [SerializeField, Min(0)] private int _uncommonWeight;
    [SerializeField, Min(0)] private int _rareWeight;
    [SerializeField, Min(0)] private int _legendaryWeight;
    [SerializeField, Min(0)] private int _odditiesWeight;

    public event Action<string> OnAbilityPurchased;

    [SerializeField] int _maxReroll;
    int _numberOfReroll;


    private void Awake()
    {
        _towerManager = FindAnyObjectByType<TowerManager>();
        _towerUIManager = FindAnyObjectByType<TowerUIManager>();
    }
    private void Start()
    {
        ResetRoroll();
        SetItemAvailableToPurchase();
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
        //Now do both ability and card. need only for car
        if (_towerManager.GetMileStoneEXP() < price)
        {
            _towerUIManager.PlayPopUpLackEssence();
            return false;

        }

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

        _towerManager.DeductMileStoneEXP();
        unlockedAbilities.Add(abilityID);

        _abilityLevelAndCostState[abilityLevel].RegisterPurchase();

        OnAbilityPurchased?.Invoke(abilityID);

        return true;
    }

    public bool IsUnlocked(string abilityID)
    {
        if (unlockedAbilities == null)
            return false;

        return unlockedAbilities.Contains(abilityID);
    }

    public bool IsAvailableToPurchase(string abilityID)
    {
        if (abilityLookup == null)
            return false;

        if (!abilityLookup.TryGetValue(abilityID, out SOStoreAbilityContent ability))
            return false;

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
        if (_itemButtonsList == null || _itemButtonsList.Count == 0)
            return;

        // Group available cards by rarity.
        Dictionary<ITEMRARITY, List<SOItemAbilityContentUI>> rarityPools =
            new Dictionary<ITEMRARITY, List<SOItemAbilityContentUI>>();

        HashSet<SOItemAbilityContentUI> purchasedItems =
            new HashSet<SOItemAbilityContentUI>(_itemPurchasedList);

        // Prevent duplicate entries in the database.
        HashSet<SOItemAbilityContentUI> addedItems =
            new HashSet<SOItemAbilityContentUI>();

        foreach (SOItemAbilityContentUI item in _itemAvailableList)
        {
            if (item == null ||
                purchasedItems.Contains(item) ||
                !addedItems.Add(item))
            {
                continue;
            }

            // CHANGE _rarity to the actual rarity field
            // in your SOItemAbilityContentUI.
            ITEMRARITY rarity = item._itemRarity;

            if (rarity == ITEMRARITY.NULL)
                continue;

            if (!rarityPools.ContainsKey(rarity))
                rarityPools[rarity] = new List<SOItemAbilityContentUI>();

            rarityPools[rarity].Add(item);
        }

        // Fill each shop slot independently.
        for (int i = 0; i < _itemButtonsList.Count; i++)
        {
            ItemAbilityButtonUI button = _itemButtonsList[i];

            if (button == null)
                continue;

            // Select an available rarity using weights.
            ITEMRARITY selectedRarity = RollAvailableRarity(rarityPools);

            if (selectedRarity == ITEMRARITY.NULL)
            {
                button.SetItemAbilityContent(_emptyItem);
                continue;
            }

            List<SOItemAbilityContentUI> pool =
                rarityPools[selectedRarity];

            // Randomly select one card within the rarity.
            int randomIndex = UnityEngine.Random.Range(0, pool.Count);

            SOItemAbilityContentUI selectedItem = pool[randomIndex];

            // Remove from temporary pool so no duplicate
            // cards appear in the same shop roll.
            pool.RemoveAt(randomIndex);

            button.SetItemAbilityContent(selectedItem);
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
            case ITEMRARITY.UNCOMMON:
                cost = _itemCostByLevel[1];
                break;
            case ITEMRARITY.RARE:
                cost = _itemCostByLevel[2];
                break;
            case ITEMRARITY.LEGENDARY:
                cost = _itemCostByLevel[3];
                break;
            case ITEMRARITY.ODDITIES:
                cost = _itemCostByLevel[4];
                break;
        }
        return cost;
    }

    private ITEMRARITY RollAvailableRarity(
        Dictionary<ITEMRARITY, List<SOItemAbilityContentUI>> rarityPools)
    {
        int totalWeight = 0;

        // Calculate total weight from rarities that
        // still have at least one available card.
        foreach (var pair in rarityPools)
        {
            if (pair.Value.Count == 0)
                continue;

            totalWeight += GetRarityWeight(pair.Key);
        }

        // No selectable cards remain.
        if (totalWeight <= 0)
            return ITEMRARITY.NULL;

        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        foreach (var pair in rarityPools)
        {
            if (pair.Value.Count == 0)
                continue;

            int weight = GetRarityWeight(pair.Key);

            if (weight <= 0)
                continue;

            if (randomValue < weight)
                return pair.Key;

            randomValue -= weight;
        }

        return ITEMRARITY.NULL;
    }
    private int GetRarityWeight(ITEMRARITY rarity)
    {
        switch (rarity)
        {
            case ITEMRARITY.COMMON:
                return _commonWeight;

            case ITEMRARITY.UNCOMMON:
                return _uncommonWeight;

            case ITEMRARITY.RARE:
                return _rareWeight;

            case ITEMRARITY.LEGENDARY:
                return _legendaryWeight;

            case ITEMRARITY.ODDITIES:
                return _odditiesWeight;

            default:
                return 0;
        }
    }
    public void MoveAbility(SOItemAbilityContentUI item)
    {
        _itemPurchasedList.Add(item);
        _itemAvailableList.Remove(item);
    }
    public void RerollItem()
    {
        if(_numberOfReroll > 0)
        {
            SetItemAvailableToPurchase();
            _numberOfReroll--;
        }
    }
    public List<SOItemAbilityContentUI> GetPurchaseItem() => _itemPurchasedList;
    public List<ItemAbilityButtonUI> GetAllCards() => _itemButtonsList;
    public void ChangeRerollValue(int val) => _numberOfReroll += val;
    public int GetNumberOfReroll() => _numberOfReroll;
    public void ResetRoroll() => _numberOfReroll = _maxReroll;
}