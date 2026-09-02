using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BallAbilityLookUpStatsUI : BaseButtonInteraction
{

    [SerializeField]StoreAbilityManager _storeAbilityManager;
    [SerializeField]BookOverlayUI _bookOverlayUI;
    SOStoreAbilityContent _content;

    [Header("UI Detail")]
    [SerializeField] private Image _thumbnailIcon;
    [SerializeField] private GameObject _lockedOverlay;
    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }
    public void SetUp(SOStoreAbilityContent content)
    {
        if (content == null) 
            return;


        _content = content;
        _thumbnailIcon.sprite = _content.icon;

        if (!_storeAbilityManager.IsUnlocked(_content.abilityID))
        {
            _lockedOverlay.SetActive(true);
            return;
        }
        _lockedOverlay.SetActive(false);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (_content == null)
            return;

        base.OnPointerEnter(eventData);
        _bookOverlayUI.SetAbilityDetail(_content);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (_content == null)
            return;

        base.OnPointerEnter(eventData);
        _bookOverlayUI.ClearAbilityDetail();
    }
}
