using System.Collections.Generic;
using UnityEngine;

public class EssencePool : MonoBehaviour
{
    public GameObject _essencePrefab;
    [SerializeField] int _essenceToSpawn;

    public List<GameObject> _essencePool = new List<GameObject>();
    bool _hasSpawned;

    private void Start()
    {
        if (!_hasSpawned)
        {
            SpawnEssence();
            _hasSpawned = true;
        }
    }

    void SpawnEssence()
    {
        for (int i = 0; i < _essenceToSpawn; i++)
        {
            GameObject _b = Instantiate(_essencePrefab, transform);
            _essencePool.Add(_b);
            _b.SetActive(false);
        }
    }

    public GameObject GetEssence()
    {
        foreach (var e in _essencePool)
        {
            if (!e.activeSelf)
            {
                e.SetActive(true);
                return e;
            }
        }
        return null;
    }
}