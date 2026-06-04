using System;
using Unity.VisualScripting;
using UnityEngine;
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

    private SOStoreAbilityContent _abilityData;
    private StoreAbilityManager _storeAbilityManager;

    private AbilityInfo _abilityInfo;

    [Header("UI Detail")]
    [SerializeField] private Image _thumbnailIcon;
    [SerializeField] private Button _viewAbilityButton;
    [SerializeField] private GameObject _lockedOverlay;
    [SerializeField] private GameObject _abilityDescription;

    private bool _abilityPurchased;

    private void Awake()
    {
        _abilityInfoPageUI = FindAnyObjectByType<AbilityInfoPageUI>();
    }

    private void Start()
    {
        _viewAbilityButton.onClick.AddListener(ViewAbility);
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
        SOStoreAbilityContent ability,
        StoreAbilityManager manager)
    {
        _abilityData = ability;
        _storeAbilityManager = manager;

        // Subscribe here if instantiated while active
        _storeAbilityManager.OnAbilityPurchased += HandleAbilityPurchased;

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

        _viewAbilityButton.interactable = available;

        _lockedOverlay.SetActive(!available);

        if (unlocked)
        {
            _viewAbilityButton.interactable = true;
        }
    }

    private void ViewAbility()
    {
        if (_abilityInfoPageUI == null || _abilityInfo == null)
            return;

        _abilityInfoPageUI.SetUpAbilityDescription(_abilityInfo);
        _abilityInfoPageUI.IsAbilityPurchased(!_abilityPurchased);
    }
    public AbilityInfo GetAbilityInfo() => _abilityInfo;
}
