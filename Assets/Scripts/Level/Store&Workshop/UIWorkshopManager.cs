using System;
using UnityEngine;
using UnityEngine.UI;

public class UIWorkshopManager : AbstractStoreUI
{
    Ball _ball;
    [SerializeField] Button _upgradeBaseDamage;

    private void Start()
    {
        _ball = FindAnyObjectByType<Ball>();
    }

}
