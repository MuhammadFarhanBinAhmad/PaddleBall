using System.Collections.Generic;
using UnityEngine;

public class EssenceManager : MonoBehaviour
{
    public static EssenceManager Instance;

    private readonly List<TowerEssence> _activeEssences = new();

    public IReadOnlyList<TowerEssence> ActiveEssences => _activeEssences;

    private void Awake()
    {
        Instance = this;
    }

    public void Register(TowerEssence essence)
    {
        if (!_activeEssences.Contains(essence))
            _activeEssences.Add(essence);
    }

    public void Unregister(TowerEssence essence)
    {
        _activeEssences.Remove(essence);
    }
}