using System.Collections;
using System.Collections.Generic;
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
    Animator _abilityAnimator;
    [SerializeField] Button _openSelectAbilityPage;
    [SerializeField] Button _closeSelectAbilityPage;
    [SerializeField] GameObject _selectAbilityOverlay;
    [SerializeField] Animator _abilitySelectAbilityAnimator;

    [Header("AbilityPage")]
    [SerializeField] Button _closePurchaseAbilityOverlay;
    [SerializeField] GameObject _abilityOverlay;
    [SerializeField] Animator _purchaseAbilityAnimator;
    [SerializeField] TextMeshProUGUI _apCardCostText;
    [SerializeField] TextMeshProUGUI _apCurrentPointText;
    [SerializeField] TextMeshProUGUI _apLeftOverPointText;

    [Header("ItemPage")]
    [SerializeField] Button _openCardPage;
    [SerializeField] Button _closeItem;
    [SerializeField] Button _rerollItem;
    [SerializeField] GameObject _cardOverlay;
    [SerializeField] Animator _cardStoreAnimator;
    [SerializeField] TextMeshProUGUI _cardCostText;
    [SerializeField] TextMeshProUGUI _currentPointText;
    [SerializeField] TextMeshProUGUI _leftOverPointText;
    [SerializeField] TextMeshProUGUI _rerollLeftText;

    public List<ItemAbilityButtonUI> _itemButton = new List<ItemAbilityButtonUI>();

    bool _pageOpen;
    private void Awake()
    {
        _storeAbilityManager = FindAnyObjectByType<StoreAbilityManager>();
        _abilityStoreLayoutUI = FindAnyObjectByType<AbilityStoreLayoutUI>();
    }

    private void Start()
    {
        // Select ability ¨ open ability page
        _explosiveAbility.onClick.AddListener(() => OpenPurchaseAbilityPage(STATUSTYPE.EXPLOSION));
        _dischargeAbility.onClick.AddListener(() => OpenPurchaseAbilityPage(STATUSTYPE.DISCHARGE));
        _toxicAbility.onClick.AddListener(() => OpenPurchaseAbilityPage(STATUSTYPE.TOXIC));
        _criticalAbility.onClick.AddListener(() => OpenPurchaseAbilityPage(STATUSTYPE.CRIT));

        _openSelectAbilityPage.onClick.AddListener(HandleAbilityFlow);
        _closeSelectAbilityPage.onClick.AddListener(HandleAbilityFlow);
        _closePurchaseAbilityOverlay.onClick.AddListener(ClosePurchaseAbilityPage);

        // Item
        _openCardPage.onClick.AddListener(ToggleCardPage);
        _closeItem.onClick.AddListener(PlayCloseCardStoreAnim);
        _rerollItem.onClick.AddListener(StartRerollCardAnim);

        _rerollLeftText.text = _storeAbilityManager.GetNumberOfReroll().ToString();

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            HandleAbilityFlow();

        if (Input.GetKeyDown(KeyCode.E))
            ToggleCardPage();
    }
    void HandleAbilityFlow()
    {
        // CASE 1: AbilityPage is open ¨ close it first
        if (_abilityOverlay.activeSelf)
        {
            CloseOverlay(_abilityOverlay, false , true);
            return;
        }

        // CASE 2: SelectAbility is open ¨ close it (only if page is already closed)
        if (_selectAbilityOverlay.activeSelf)
        {
            PlayCloseSelectAbilityAnim();
            return;
        }

        // CASE 3: Nothing open ¨ open SelectAbility first
        OpenOverlay(_selectAbilityOverlay);
        _abilitySelectAbilityAnimator.SetTrigger("OpenAbilityStore");
    }
    void OpenPurchaseAbilityPage(STATUSTYPE type)
    {
        _storeAbilityManager.SetUpAbility(type);
        _abilityStoreLayoutUI.BuildStore(type);

        StartCoroutine(PlayOpenPurchaseAbilityAnim());
    }
    IEnumerator PlayOpenPurchaseAbilityAnim()
    {
        _abilitySelectAbilityAnimator.SetTrigger("SelectedAbility");
        yield return null;

        AnimatorStateInfo state =
            _abilitySelectAbilityAnimator.GetCurrentAnimatorStateInfo(0);

        yield return new WaitForSecondsRealtime(state.length - .2f);
        OpenOverlay(_abilityOverlay);
        _purchaseAbilityAnimator.SetTrigger("OpenAbilityPage");
        state =
        _purchaseAbilityAnimator.GetCurrentAnimatorStateInfo(0);

        yield return new WaitForSecondsRealtime(state.length - .2f);
        //// must open select first
        //if (!_selectAbilityOverlay.activeSelf)
        //{
        //    OpenOverlay(_selectAbilityOverlay);
        //}

    }
    IEnumerator PlayClosePurchaseAbilityAnim()
    {
        _purchaseAbilityAnimator.SetTrigger("CloseAbilityPage");

        yield return null;

        AnimatorStateInfo state =
            _purchaseAbilityAnimator.GetCurrentAnimatorStateInfo(0);

        // must open select first
        if (_selectAbilityOverlay.activeSelf)
        {
            OpenOverlay(_selectAbilityOverlay);
        }
        _abilitySelectAbilityAnimator.SetTrigger("OpenAbilityStore");

        yield return new WaitForSecondsRealtime(state.length);
        CloseOverlay(_abilityOverlay, false, true);
    }
    void ClosePurchaseAbilityPage()
    {
        StartCoroutine(PlayClosePurchaseAbilityAnim());
    }
    void ToggleCardPage()
    {
        if (_cardOverlay.activeSelf)
        {
            CloseOverlay(_cardOverlay);
            return;
        }

        OpenOverlay(_cardOverlay);
        _cardStoreAnimator.SetTrigger("OpenCardStore");
    }
    public void PlayCloseCardStoreAnim()
    {
        StartCoroutine(CloseAfterAnimation());
    }
    public void PlayCloseSelectAbilityAnim()
    {
        StartCoroutine(CloseSelectAbilityAnimation());
    }
    private IEnumerator CloseAfterAnimation()
    {
        _cardStoreAnimator.SetTrigger("CloseCardStore");

        yield return null;

        AnimatorStateInfo state =
            _cardStoreAnimator.GetCurrentAnimatorStateInfo(0);

        yield return new WaitForSecondsRealtime(state.length + 0.5f);
        base.CloseOverlay(_cardOverlay);
    }
    private IEnumerator CloseSelectAbilityAnimation()
    {
        _abilitySelectAbilityAnimator.SetTrigger("CloseAbilityStore");

        yield return null;

        AnimatorStateInfo state =
            _abilitySelectAbilityAnimator.GetCurrentAnimatorStateInfo(0);

        yield return new WaitForSecondsRealtime(state.length + 0.5f);
        CloseOverlay(_selectAbilityOverlay);
    }
    public void StartRerollCardAnim()
    {
        if (_storeAbilityManager.GetNumberOfReroll() > 0)
        {
            StartCoroutine(RerollCardAnim());
        }
    }
    IEnumerator RerollCardAnim()
    {

        _cardStoreAnimator.SetTrigger("RerollCard");
        AnimatorStateInfo state =
        _cardStoreAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSecondsRealtime(state.length - .25f);
        _storeAbilityManager.RerollItem();
        UpdateRerollText();
        ResetItems();
    }
    public void CalculatePriceCalculation(int cost, int totalpoint)
    {
        _cardCostText.text = cost.ToString();
        _currentPointText.text = "- " + totalpoint.ToString();
        _leftOverPointText.text = (totalpoint - cost).ToString();

        _apCardCostText.text = cost.ToString();
        _apCurrentPointText.text = "- " + totalpoint.ToString();
        _apLeftOverPointText.text = (totalpoint - cost).ToString();
    }
    public void ResetPriceCalculation()
    {
        _cardCostText.text = "";
        _currentPointText.text = "";
        _leftOverPointText.text = "";

        _apCardCostText.text = "";
        _apCurrentPointText.text = "";
        _apLeftOverPointText.text = "";
    }
    public void ResetItems()
    {
        for (int i = 0; i < _itemButton.Count; i++)
        {
            _itemButton[i].ResetButton();
        }
    }
    public void UpdateRerollText() => _rerollLeftText.text = _storeAbilityManager.GetNumberOfReroll().ToString();

}
