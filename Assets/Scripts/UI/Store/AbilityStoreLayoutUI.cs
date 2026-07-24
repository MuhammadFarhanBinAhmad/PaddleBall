using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;


[System.Serializable]
public class AbilityButtonLevel
{
    public List<SOStoreAbilityContent> buttons;
}


public class AbilityStoreLayoutUI : AbstractStoreUI
{
    [SerializeField]AbilityInfoPageUI _abilityInfoPageUI;
    StoreAbilityManager _storeAbilityManager;

    [Header("UI")]
    [SerializeField] Transform _contentParent;
    [SerializeField] BallAbilityButtonUI _abilityButtonPrefab;

    [Header("Data")]
    List<SOStoreAbilityContent> abilityList = new List<SOStoreAbilityContent>();
    [SerializeField]
    AbilityButtonLevel[] levelButtons = new AbilityButtonLevel[4];

    List<BallAbilityButtonUI>[] spawnedButtonsLevels = new List<BallAbilityButtonUI>[4];
    public GameObject _lvl0AbilityButton;
    public List<GameObject> _lvl1AbilityButton = new List<GameObject>();
    public List<GameObject> _lvl2AbilityButton = new List<GameObject>();
    public List<GameObject> _lvl3AbilityButton = new List<GameObject>();


    private void Awake()
    {
        _storeAbilityManager = FindAnyObjectByType<StoreAbilityManager>();

    }


    public void BuildStore(STATUSTYPE _type)
    {

        // Get abilities
        abilityList = _storeAbilityManager.GetAbilityList(_type);

        // Group abilities by level
        foreach (SOStoreAbilityContent ability in abilityList)
        {
            int level = ability.ability_Level;
            levelButtons[level].buttons.Add(ability);
        }

        PopulateLevel0();
        PopulateLevel(_lvl1AbilityButton, levelButtons[1].buttons);
        //PopulateLevel(_lvl2AbilityButton, levelButtons[2].buttons);
        //PopulateLevel(_lvl3AbilityButton, levelButtons[3].buttons);
    }
    void PopulateLevel0()
    {
        if (levelButtons[0].buttons.Count == 0)
        {
            _lvl0AbilityButton.SetActive(false);
            return;
        }

        _lvl0AbilityButton.SetActive(true);

        BallAbilityButtonUI ui =
            _lvl0AbilityButton.GetComponent<BallAbilityButtonUI>();

        ui.Setup(levelButtons[0].buttons[0]);
    }
    void PopulateLevel(List<GameObject> buttons,
                   List<SOStoreAbilityContent> abilities)
    {
        int count = buttons.Count;

        for (int i = 0; i < count; i++)
        {
            buttons[i].SetActive(true);

            BallAbilityButtonUI ui =
                buttons[i].GetComponent<BallAbilityButtonUI>();

            ui.Setup(abilities[i]);
        }
    }

    public void RefreshAll()
    {
        for (int level = 0; level < 4; level++)
        {
            foreach (BallAbilityButtonUI button in spawnedButtonsLevels[level])
            {
                button.Refresh();
            }
        }
    }
}
