using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PurchasedCardInfoButton : BaseButtonInteraction
{
    SOItemAbilityContentUI _cardDetails;
    PurchasedCardContent _purchasedCardContent;
    [SerializeField] Image _icon;

    private void Awake()
    {
        _purchasedCardContent = FindAnyObjectByType<PurchasedCardContent>();
    }
    public bool IsEmpty() => _cardDetails == null;
    public void SetCardDetail(SOItemAbilityContentUI stats)
    {
        if(stats == null)
        {
            _icon.color = Color.clear;
            return;
        }
        else
        {
            _cardDetails = stats;
            _icon.sprite = _cardDetails.icon;
        }

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if(_cardDetails!=null)
            _purchasedCardContent.SetCard(_cardDetails);
    }
}
