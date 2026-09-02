using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ItemAbilityButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    TowerManager _towerManager;
    TowerUIManager _towerUIManager;
    AbilityManager _abilityManager;
    StoreOverlayUI _storeOverlayUI;
    OnTheHouse _onTheHouse;
    //ViewItemAbilityButtonUI _viewItemAbilityButtonUI;
    [SerializeField]StoreAbilityManager _storeAbilityManager;
    internal SOItemAbilityContentUI _itemAbilityContent { get;private set; }

    [Header("UI Detail")]
    [SerializeField] Image _icon;
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _descriptionText;
    [SerializeField] TMP_Text _costText;
    [SerializeField] GameObject _backCard;
    [SerializeField] Button _selectButton;
    [SerializeField] Image _rarityEmblem;
    [SerializeField] Sprite[] _emblem;
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
        switch (_itemAbilityContent._itemRarity)
        {
            case ITEMRARITY.COMMON:
                {
                    _rarityEmblem.sprite = _emblem[0];
                    break;
                }
            case ITEMRARITY.UNCOMMON:
                {
                    _rarityEmblem.sprite = _emblem[1];
                    break;
                }
            case ITEMRARITY.RARE:
                {
                    _rarityEmblem.sprite = _emblem[2];
                    break;
                }
            case ITEMRARITY.LEGENDARY:
                {
                    _rarityEmblem.sprite = _emblem[3];
                    break;
                }
        }
    }

    public void ResetButton()
    {
        _selectButton.interactable = true;
        _isPurchase = false;
        _backCard.SetActive(false);
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

        if(_onTheHouse != null)
        {
            if (!_onTheHouse._itemPurchase)
            {
                PurchaseCard();
                _onTheHouse._itemPurchase = true;
                return; 
            }
        }

        if (_towerManager._currentPureEssence >= _purchaseCost)
        {
            PurchaseCard();
        }
        else
        {
            _towerUIManager.PlayPopUpLackEssence();
            print("insufficnet");
        }
    }
    void PurchaseCard()
    {

        if(_onTheHouse == null)
        {
            _towerManager.DeductPureEssence(_purchaseCost);
            _abilityManager.AddAbility(GetAbilityToSpawn());
            DeactiveButton();
            _isPurchase = true;
            _towerUIManager.UpdateEssenceUI();
        }
        else
        {
            print("Got house");
            if (!_onTheHouse._itemPurchase)
            {
                _abilityManager.AddAbility(GetAbilityToSpawn());
                DeactiveButton();
                _isPurchase = true;
                _onTheHouse._itemPurchase=_isPurchase;
            }
        }

        _storeAbilityManager.MoveAbility(_itemAbilityContent);
        _itemAbilityContent = null;
    }    
    public void SetOnTheHouse(OnTheHouse oth) => _onTheHouse = oth;

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
