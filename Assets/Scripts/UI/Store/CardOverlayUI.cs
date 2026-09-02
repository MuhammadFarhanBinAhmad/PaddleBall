using System.Collections.Generic;
using UnityEngine;

public class CardOverlayUI : MonoBehaviour
{
    [SerializeField] private List<PurchasedCardInfoButton> cardDetailButtons = new();

    [SerializeField] Transform _parent;

    StoreAbilityManager _storeAbilityManager;

    private void Awake()
    {
        _storeAbilityManager = FindAnyObjectByType<StoreAbilityManager>();

        for (int i = 0; i < _parent.childCount; i++)
        {
            Transform buttons = _parent.GetChild(i);
            foreach (Transform child in buttons)
            {
                cardDetailButtons.Add(child.GetComponent<PurchasedCardInfoButton>());
            }
        }
    }
    private void OnEnable()
    {
        if(_storeAbilityManager.GetPurchaseItem().Count == 0)
            return;

        List<SOItemAbilityContentUI> _purchasedItem = _storeAbilityManager.GetPurchaseItem();

        for (int i = 0; i < _purchasedItem.Count; i++)
        {
            if(i < cardDetailButtons.Count)
            {
                if (cardDetailButtons[i].IsEmpty())
                {
                    cardDetailButtons[i].SetCardDetail(_purchasedItem[i]);
                }
                else
                {
                    continue;
                }
            }
        }

    }
}
