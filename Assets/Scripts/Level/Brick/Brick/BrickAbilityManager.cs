using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AbilityObject
{
    public ABILITY _ability;
    public GameObject _gameObject;
}
public class BrickAbilityManager : MonoBehaviour
{
    public List<AbilityObject> _abilityList = new List<AbilityObject>();

    public void ActivateAbilities(ABILITY _ABS)
    {
        foreach (AbilityObject ability in _abilityList)
        {
            bool isActive = (_ABS & ability._ability) != 0;

            ability._gameObject.SetActive(isActive);
        }
    }
}
