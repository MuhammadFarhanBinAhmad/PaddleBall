using System.Collections;
using UnityEngine;

public class BossEssenceOrb : MonoBehaviour
{
    EssencePool _essencePool;
    TimeManager _timeManager;

    [SerializeField] float _spawnInterval = 0.1f;
    [SerializeField] Transform _spawnPoint;

    Coroutine _spawnRoutine;

    private void Awake()
    {
        _essencePool = FindAnyObjectByType<EssencePool>();
        _timeManager = FindAnyObjectByType<TimeManager>();
    }

    private void OnEnable()
    {
        if (_timeManager != null)
            _timeManager._onBossDefeated += StartTimelapseSpawn;
    }

    private void OnDisable()
    {
        if (_timeManager != null)
            _timeManager._onBossDefeated -= StartTimelapseSpawn;
    }

    void StartTimelapseSpawn()
    {
        if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine);

        _spawnRoutine = StartCoroutine(SpawnEssenceUntilDayEnds());
    }

    IEnumerator SpawnEssenceUntilDayEnds()
    {
        while (_timeManager != null && _timeManager.GetCurrentDayDuration() > 1f)
        {
            GameObject essence = _essencePool.GetEssence();
            essence.GetComponent<TowerEssence>().SetToAutoAttract();
            if (essence != null)
            {
                essence.transform.position = _spawnPoint != null ? _spawnPoint.position : transform.position;
                essence.SetActive(true);
            }

            yield return new WaitForSeconds(_spawnInterval);
        }

        _spawnRoutine = null;
    }
}