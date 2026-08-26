using UnityEngine;

public class OnTheHouse : ABSAbility
{
    public bool _itemPurchase;

    StoreAbilityManager _manager;
    TimeManager _timeManager;

    private void Awake()
    {
        _manager = FindAnyObjectByType<StoreAbilityManager>();
        _timeManager = FindAnyObjectByType<TimeManager>();

        foreach (ItemAbilityButtonUI i in _manager.GetAllCards())
        {
            i.SetOnTheHouse(this);
        }

        _timeManager._dayPass += ResetPurchase;

    }
    private void OnDestroy()
    {
        _timeManager._dayPass -= ResetPurchase;
    }
    public void ResetPurchase() => _itemPurchase = false;
}
