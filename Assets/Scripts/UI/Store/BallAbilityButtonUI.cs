using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityInfo
{
    public Sprite _icon;
    public string _titleText;
    public string _descriptionText;
    public int _cost;
    public int ability_Level;
    public string ID;
    public SOAbilityEffect ability_ToSpawn;

    public AbilityInfo(
    Sprite icon,
    string title,
    string description,
    int cost,
    int level,
    string id,
    SOAbilityEffect ability)
    {
        _icon = icon;
        _titleText = title;
        _descriptionText = description;
        _cost = cost;
        ability_Level = level;
        ID = id;
        ability_ToSpawn = ability;
    }
}

public class BallAbilityButtonUI : BaseButtonInteraction
{
    private AbilityInfoPageUI _abilityInfoPageUI;
    StoreOverlayUI _storeOverlayUI;
    private SOStoreAbilityContent _abilityData;
    TowerManager _towerManager;
    [SerializeField]private StoreAbilityManager _storeAbilityManager;

    private AbilityInfo _abilityInfo;

    [Header("UI Detail")]
    [SerializeField] private Image _thumbnailIcon;
    [SerializeField] private GameObject _lockedOverlay;
    [SerializeField] private GameObject _abilityDescription;
    Button _button;

    private bool _abilityPurchased;

    private void Awake()
    {
        _abilityInfoPageUI = FindAnyObjectByType<AbilityInfoPageUI>();
        _button = GetComponent<Button>();
        _storeOverlayUI = FindAnyObjectByType<StoreOverlayUI>();
        _towerManager = FindAnyObjectByType<TowerManager>();
    }

    private void OnEnable()
    {
        if (_storeAbilityManager != null)
        {
            _storeAbilityManager.OnAbilityPurchased += HandleAbilityPurchased;
        }
    }

    private void OnDisable()
    {
        if (_storeAbilityManager != null)
        {
            _storeAbilityManager.OnAbilityPurchased -= HandleAbilityPurchased;
        }
    }

    public void Setup(
        SOStoreAbilityContent ability)
    {
        _abilityData = ability;

        _thumbnailIcon.sprite = _abilityData.icon;

        _abilityInfo = new AbilityInfo(
            _abilityData.icon,
            _abilityData.ability_Name,
            _abilityData.ability_Description,
            _storeAbilityManager.GetAbilityCost(_abilityData.ability_Level),
            _abilityData.ability_Level,
            _abilityData.abilityID,
            _abilityData.ability_ToSpawn
        );
        Refresh();
    }


    private void HandleAbilityPurchased(string purchasedID)
    {
        if (_abilityData == null)
            return;

        if (_abilityData.abilityID == purchasedID)
        {
            _abilityPurchased = true;
        }

        Refresh();
    }

    public void Refresh()
    {
        if (_storeAbilityManager == null || _abilityData == null)
            return;

        bool unlocked =
            _storeAbilityManager.IsUnlocked(_abilityData.abilityID);

        bool available =
            _storeAbilityManager.IsAvailableToPurchase(_abilityData.abilityID);

        bool canBuy =
            _storeAbilityManager.CanPurchase(_abilityData.abilityID);

        _abilityPurchased = unlocked;


        _lockedOverlay.SetActive(!available);

        if (unlocked)
        {
            _button.interactable = true;
        }
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        _abilityInfoPageUI.SetUpAbilityDescription(_abilityInfo , _button);
        _storeOverlayUI.CalculatePriceCalculation(_abilityInfo._cost,
                                                    _towerManager._currentPureEssence);
        //_abilityInfoPageUI.IsAbilityPurchased(!_abilityPurchased);

    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        _abilityInfoPageUI.ClearDescription();
        _storeOverlayUI.ResetPriceCalculation();
    }
}
