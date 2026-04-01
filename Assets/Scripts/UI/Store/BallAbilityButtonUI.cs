using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BallAbilityButtonUI : MonoBehaviour
{
    AbilityManager _abilityManager;
    AbilityStoreUI _abilityStoreUI;

    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private Button _purchaseButton;
    [SerializeField] private GameObject _lockedOverlay;
    bool _abilityPurchased;

    private SOStoreAbilityContent _abilityData;
    private StoreAbilityManager _storeAbilityManager;

    public Action OnAbilityPurchase;

    private void Start()
    {
        _abilityManager = FindAnyObjectByType<AbilityManager>();
        _abilityStoreUI = FindAnyObjectByType<AbilityStoreUI>();

        OnAbilityPurchase += _abilityStoreUI.RefreshAll;
    }
    private void OnDestroy()
    {
        OnAbilityPurchase -= _abilityStoreUI.RefreshAll;
    }
    public void Setup(SOStoreAbilityContent ability, StoreAbilityManager manager)
    {
        _abilityData = ability;
        _storeAbilityManager = manager;
        _icon.sprite = _abilityData.icon;
        _nameText.text = _abilityData.ability_Name;
        _descriptionText.text = _abilityData.ability_Description;
        _costText.text = _storeAbilityManager.GetAbilityCost(ability.ability_Level).ToString();

        _purchaseButton.onClick.RemoveAllListeners();
        _purchaseButton.onClick.AddListener(OnPurchaseClicked);

        Refresh();
    }

    public void Refresh()
    {
        if (_abilityPurchased)
            return;

        bool unlocked = _storeAbilityManager.IsUnlocked(_abilityData.abilityID);
        bool AvailableToPurchase = _storeAbilityManager.IsAvailableToPurchase(_abilityData.abilityID);
        bool canBuy = _storeAbilityManager.CanPurchase(_abilityData.abilityID);

        print(_abilityData.ability_Name);
        print("AvailableToPurchase: " + AvailableToPurchase);

        _purchaseButton.interactable = canBuy;
        _lockedOverlay.SetActive(!canBuy && !unlocked);

        if (unlocked)
        {
            _costText.text = "Owned";
        }
        else
        {
            _costText.text = _storeAbilityManager.GetAbilityCost(_abilityData.ability_Level).ToString();
        }
    }

    private void OnPurchaseClicked()
    {
        _abilityManager.AddAbility(_abilityData.ability_ToSpawn);
        if (_storeAbilityManager.PurchaseAbility(_abilityData.abilityID))
        {
            OnAbilityPurchase?.Invoke();
            _abilityPurchased = true;
        }
    }
}
