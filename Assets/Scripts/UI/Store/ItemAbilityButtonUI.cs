using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class ItemAbilityButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    TowerManager _towerManager;
    TowerUIManager _towerUIManager;
    AbilityManager _abilityManager;
    StoreOverlayUI _storeOverlayUI;
    //ViewItemAbilityButtonUI _viewItemAbilityButtonUI;
    [SerializeField]StoreAbilityManager _storeAbilityManager;
    internal SOItemAbilityContentUI _itemAbilityContent { get;private set; }

    [Header("UI Detail")]
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _costText;

    [SerializeField] private Button _selectButton;
    internal int _purchaseCost { get; private set; }
    bool _isPurchase = false;

    private void Awake()
    {
        //_viewItemAbilityButtonUI = FindAnyObjectByType<ViewItemAbilityButtonUI>();
        _towerManager = FindAnyObjectByType<TowerManager>();
        _abilityManager = FindAnyObjectByType<AbilityManager>();
        _storeAbilityManager = FindAnyObjectByType<StoreAbilityManager>();
        _towerUIManager = FindAnyObjectByType<TowerUIManager>();
        _storeOverlayUI = FindAnyObjectByType<StoreOverlayUI>();
    }
    private void Start()
    {
        _selectButton.onClick.AddListener(PurchaseItem);
    }
    public void SetItemAbilityContent(SOItemAbilityContentUI content)
    {

        _itemAbilityContent = content;
        SetItemButton();
    }
    public void SetItemButton()
    {
        _icon.sprite = _itemAbilityContent.icon;
        _nameText.text = _itemAbilityContent.ability_Name.ToString();
        _descriptionText.text = _itemAbilityContent.ability_Description.ToString();
        _purchaseCost = _storeAbilityManager.GetItemCost(_itemAbilityContent._itemRarity);
        _costText.text = _purchaseCost.ToString();
    }

    public void ResetButton()
    {
        _selectButton.interactable = true;
        _isPurchase = false;
    }
    public void DeactiveButton()
    {
        _selectButton.interactable = false;
    }
    public SOAbilityEffect GetAbilityToSpawn() => _itemAbilityContent.ability_ToSpawn;
    public void PurchaseItem()
    {
        if(_isPurchase)
            return;


        if (_towerManager._currentPureEssence >= _purchaseCost)
        {
            _towerManager.DeductPureEssence(_purchaseCost);
            _abilityManager.AddAbility(GetAbilityToSpawn());
            DeactiveButton();
            //_viewItemAbilityButtonUI.SetContentToNull();
            _isPurchase = true;
            _towerUIManager.UpdateEssenceUI();
        }
        else
        {
            print("insufficnet");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _storeOverlayUI.CalculatePriceCalculation(_storeAbilityManager.GetItemCost(_itemAbilityContent._itemRarity),
                                                  _towerManager._currentPureEssence);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _storeOverlayUI.ResetPriceCalculation();
    }
}
