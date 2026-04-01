using System.Collections.Generic;
using UnityEngine;

public class AbilityStoreUI : MonoBehaviour
{

    StoreAbilityManager _storeAbilityManager;
    [SerializeField] Transform _contentParent;
    [SerializeField] BallAbilityButtonUI _abilityButtonPrefab;
    [SerializeField] List<SOStoreAbilityContent> abilityList = new List<SOStoreAbilityContent>();

    private readonly List<BallAbilityButtonUI> spawnedButtons = new List<BallAbilityButtonUI>();

    private void Start()
    {
        _storeAbilityManager = FindAnyObjectByType<StoreAbilityManager>();
        BuildStore();
    }
    public void BuildStore()
    {
        foreach (Transform child in _contentParent)
        {
            Destroy(child.gameObject);
        }

        spawnedButtons.Clear();

        abilityList = _storeAbilityManager.GetAbilityList();

        foreach (SOStoreAbilityContent ability in abilityList)
        {
            BallAbilityButtonUI button = Instantiate(_abilityButtonPrefab, _contentParent);
            button.Setup(ability, _storeAbilityManager);
            spawnedButtons.Add(button);
        }
    }

    public void RefreshAll()
    {
        foreach (BallAbilityButtonUI button in spawnedButtons)
        {
            button.Refresh();
        }
    }

}
