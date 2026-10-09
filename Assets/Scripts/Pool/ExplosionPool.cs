using System.Collections.Generic;
using UnityEngine;

public class ExplosionPool : MonoBehaviour
{
    public GameObject _explosionPrefab;
    public List<GameObject> _explosionPrefabPool = new List<GameObject>();
    [SerializeField] int _explosionToSpawn;

    AbilityManager _abilityManager;

    private void OnEnable()
    {
        SpawnAllExplosion();
    }
    private void Awake()
    {
        _abilityManager = FindAnyObjectByType<AbilityManager>();
    }
    void SpawnAllExplosion()
    {
        for (int i = 0; i < _explosionToSpawn; i++)
        {
            GameObject _e = Instantiate(_explosionPrefab, transform);

            _e.transform.parent = transform;
            _explosionPrefabPool.Add(_e);
            _e.SetActive(false);
        }
    }
    public GameObject GetExplosion()
    {
        foreach (var e in _explosionPrefabPool)
        {
            if (!e.activeSelf)
            {
                e.SetActive(true);
                return e;
            }
        }
        return null;
    }
    public void SpawnExplosion(ExplosionContext context,Transform pos)
    {

        GameObject explosionGO = GetExplosion();
        explosionGO.transform.position = transform.position;
        var ed = explosionGO.GetComponent<ExplosionDamage>();
        if (ed == null) return;

        ExplosionContext ectx = new ExplosionContext
        {
            _source = gameObject,
            _position = pos.position,
            _statusEffect = null
        };
        ectx._Stats[STATID.BASE_DAMAGE] = context._Stats[STATID.BASE_DAMAGE];
        ectx._Stats[STATID.EXPLOSION_RADIUS] = context._Stats[STATID.EXPLOSION_RADIUS];

        // Let other abilities modify the explosion data
        _abilityManager.ApplyExplosionModifiers(null, ectx);
        ed.Initialize(ectx, true);
    }    

}
