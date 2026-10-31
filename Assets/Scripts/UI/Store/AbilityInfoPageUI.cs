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
    StatManager _statManager;

    [SerializeField] private Image _icon;
    [SerializeField] Button _button;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _costText;
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
        _statManager = FindAnyObjectByType<StatManager>();
    }
    private void Start()
    {
        OnAbilityPurchase += _abilityStoreUI.RefreshAll;
        OnAbilityPurchase += _towerUIManager.UpdateEssenceUI;
    }
    private void OnDestroy()
    {
        OnAbilityPurchase -= _abilityStoreUI.RefreshAll;
        OnAbilityPurchase -= _towerUIManager.UpdateEssenceUI;

    }
    public void SetUpAbilityDescription(AbilityInfo abilityInfo, Button button)
    {
        _icon.sprite = abilityInfo._icon;
        _titleText.text = abilityInfo._titleText;
        _descriptionText.text = abilityInfo._descriptionText;
        _costText.text = abilityInfo._cost.ToString();
        ID = abilityInfo.ID;
        ability_ToSpawn = abilityInfo.ability_ToSpawn;
        if (_button != null)
            _button.onClick.RemoveListener(PurchaseAbility);
        _button = button;
        _button.onClick.AddListener(PurchaseAbility);
    }
    public void ClearDescription()
    {
        _icon.sprite = null;
        _titleText.text = null;
        _descriptionText.text = null;
        _costText.text = null;
        ID = null;
        ability_ToSpawn = null;
    }
    public void PurchaseAbility()
    {
        print("hit");
        if (_storeAbilityManager.PurchaseAbility(ID))
        {
            _statManager.AddBallSpell();

            switch(ability_ToSpawn._statusType)
            {
                case STATUSTYPE.EXPLOSION:
                    {
                        _statManager.AddTotalExplosionSpell();
                        break;
                    }
                case STATUSTYPE.DISCHARGE:
                    {
                        _statManager.AddTotalDischargeSpell();
                        break;
                    }
                case STATUSTYPE.CRIT:
                    {
                        _statManager.AddTotalCritSpell();
                        break;
                    }
                case STATUSTYPE.TOXIC:
                    {
                        _statManager.AddTotalToxicSpell();
                        break;
                    }
            }
            _abilityManager.AddAbility(ability_ToSpawn);
            IsAbilityPurchased(false);
            OnAbilityPurchase?.Invoke();
        }
    }
    public void IsAbilityPurchased(bool purchased)
    {
        _button.interactable = purchased;
    }

}
