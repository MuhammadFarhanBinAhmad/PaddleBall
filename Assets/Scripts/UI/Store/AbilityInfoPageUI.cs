using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityInfoPageUI : MonoBehaviour
{
    TowerUIManager _towerUIManager;
    AbilityManager _abilityManager;
    AbilityStoreLayoutUI _abilityStoreUI;
    StoreAbilityManager _storeAbilityManager;

    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private Button _purchaseButton;
    [SerializeField] private GameObject _lockedOverlay;

    string ID;
    SOAbilityEffect ability_ToSpawn;

    public Action OnAbilityPurchase;

    private void Awake()
    {
        _towerUIManager = FindAnyObjectByType<TowerUIManager>();
        _abilityStoreUI = FindAnyObjectByType<AbilityStoreLayoutUI>();
        _abilityManager = FindAnyObjectByType<AbilityManager>();
        _storeAbilityManager = FindAnyObjectByType<StoreAbilityManager>();

    }
    private void Start()
    {
        OnAbilityPurchase += _abilityStoreUI.RefreshAll;
        OnAbilityPurchase += _towerUIManager.UpdateEssenceUI;

        _purchaseButton.onClick.AddListener(PurchaseAbility);
    }
    private void OnDestroy()
    {
        OnAbilityPurchase -= _abilityStoreUI.RefreshAll;
        OnAbilityPurchase -= _towerUIManager.UpdateEssenceUI;

    }
    public void SetUpAbilityDescription(AbilityInfo abilityInfo)
    {
        _icon.sprite = abilityInfo._icon;
        _titleText.text = abilityInfo._titleText;
        _descriptionText.text = abilityInfo._descriptionText;
        _costText.text = "Cost : " + abilityInfo._cost.ToString();
        ID = abilityInfo.ID;
        ability_ToSpawn = abilityInfo.ability_ToSpawn;
    }
    public void PurchaseAbility()
    {
        if (_storeAbilityManager.PurchaseAbility(ID))
        {
            _abilityManager.AddAbility(ability_ToSpawn);
            IsAbilityPurchased(false);
            OnAbilityPurchase?.Invoke();
        }
    }
    public void IsAbilityPurchased(bool purchased)
    {
        _purchaseButton.interactable = purchased;
    }

}
