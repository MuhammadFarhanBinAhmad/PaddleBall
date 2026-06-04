using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreOverlayUI : BaseOverLayInteraction
{

    StoreAbilityManager _storeAbilityManager;
    AbilityStoreLayoutUI _abilityStoreLayoutUI;

    [Header("SelectAbilityPage")]
    [SerializeField] Button _explosiveAbility;
    [SerializeField] Button _dischargeAbility;
    [SerializeField] Button _toxicAbility;
    [SerializeField] Button _criticalAbility;

    [SerializeField] Button _closeSelectAbilityPage;
    [SerializeField] GameObject _selectAbilityOverlay;

    [Header("AbilityPage")]
    [SerializeField] TextMeshProUGUI _abilityTypeText;
    [SerializeField] Button _closePurchaseAbilityOverlay;
    [SerializeField] GameObject _purchaseAbilityOverlay;

    [Header("Item")]
    [SerializeField] Button _closeItem;
    [SerializeField] Button _rerollItem;
    [SerializeField] GameObject _itemOverlay;

    bool _pageOpen;
    private void Awake()
    {
        _storeAbilityManager = FindAnyObjectByType<StoreAbilityManager>();
        _abilityStoreLayoutUI = FindAnyObjectByType<AbilityStoreLayoutUI>();
    }

    private void Start()
    {
        // Select ability Å® open ability page
        _explosiveAbility.onClick.AddListener(() => OpenPurchaseAbilityPage(STATUSTYPE.EXPLOSION));
        _dischargeAbility.onClick.AddListener(() => OpenPurchaseAbilityPage(STATUSTYPE.DISCHARGE));
        _toxicAbility.onClick.AddListener(() => OpenPurchaseAbilityPage(STATUSTYPE.TOXIC));
        _criticalAbility.onClick.AddListener(() => OpenPurchaseAbilityPage(STATUSTYPE.CRIT));

        _closeSelectAbilityPage.onClick.AddListener(HandleAbilityFlow);
        _closePurchaseAbilityOverlay.onClick.AddListener(ClosePurchaseAbilityPage);

        // Item
        _closeItem.onClick.AddListener(() => CloseOverlay(_itemOverlay));

        _rerollItem.onClick.AddListener(_storeAbilityManager.RerollItem);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            HandleAbilityFlow();

        if (Input.GetKeyDown(KeyCode.E))
            ToggleItem();
    }
    void HandleAbilityFlow()
    {
        // CASE 1: AbilityPage is open Å® close it first
        if (_purchaseAbilityOverlay.activeSelf)
        {
            CloseOverlay(_purchaseAbilityOverlay, false , true);
            return;
        }

        // CASE 2: SelectAbility is open Å® close it (only if page is already closed)
        if (_selectAbilityOverlay.activeSelf)
        {
            CloseOverlay(_selectAbilityOverlay);
            return;
        }

        // CASE 3: Nothing open Å® open SelectAbility first
        OpenOverlay(_selectAbilityOverlay);
    }
    void OpenPurchaseAbilityPage(STATUSTYPE type)
    {
        _storeAbilityManager.SetUpAbility(type);
        _abilityStoreLayoutUI.BuildStore(type);

        switch (type)
        {
            case STATUSTYPE.CRIT:
                _abilityTypeText.text = "Critical";
                break;
            case STATUSTYPE.EXPLOSION:
                _abilityTypeText.text = "Explosive";
                break;
            case STATUSTYPE.TOXIC:
                _abilityTypeText.text = "Toxic";
                break;
            case STATUSTYPE.DISCHARGE:
                _abilityTypeText.text = "Discharge";
                break;
        }


        // must open select first
        if (!_selectAbilityOverlay.activeSelf)
        {
            OpenOverlay(_selectAbilityOverlay);
        }

        OpenOverlay(_purchaseAbilityOverlay);
    }
    void ClosePurchaseAbilityPage()
    {
        // must open select first
        if (_selectAbilityOverlay.activeSelf)
        {
            OpenOverlay(_selectAbilityOverlay);
        }

        CloseOverlay(_purchaseAbilityOverlay, false , true);
    }
    void ToggleItem()
    {
        if (_itemOverlay.activeSelf)
        {
            CloseOverlay(_itemOverlay);
            return;
        }

        OpenOverlay(_itemOverlay);
    }
}
