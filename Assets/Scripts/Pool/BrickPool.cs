using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BrickPool : MonoBehaviour
{
    BrickGenerator _brickGenerator;
    TimeManager _timeManager;
    BossManager _bossManager;

    public GameObject _brickPrefab;

    public List<GameObject> _brickPool = new List<GameObject>();
    public List<GameObject> _activeBrick = new List<GameObject>();

    [SerializeField]int _brickToSpawn;

    private void Awake()
    {
        _brickGenerator = FindAnyObjectByType<BrickGenerator>();
        _timeManager = FindAnyObjectByType<TimeManager>();
        _bossManager = FindAnyObjectByType<BossManager>();

        SpawnBrick();

    }
    void SpawnBrick()
    {
        for (int i=0; i < _brickToSpawn;i++)
        {
            GameObject _b = Instantiate(_brickPrefab,transform);
            _b.transform.parent = transform;
            _brickPool.Add(_b);
            _b.SetActive(false);
        }
    }

    public GameObject GetBrick()
    {
        foreach (var brick in _brickPool)
        {
            if (!brick.activeSelf)
            {
                brick.transform.localScale = Vector3.one;
                brick.SetActive(true);
                return brick;
            }
        }
        return null;
    }

    public GameObject GetNearestActiveBrick(Vector2 position, float maxDistance = Mathf.Infinity)
    {
        GameObject nearest = null;
        float minDist = maxDistance;

        for (int i = 0; i < _activeBrick.Count; i++)
        {
            GameObject b = _activeBrick[i];
            if (b == null || !b.activeSelf) continue;

            float d = Vector2.Distance(position, b.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = b;
            }
        }
        return nearest;
    }
    public void PlaceActiveBrickInList(GameObject brick) => _activeBrick.Add(brick);
    public void RemoveActiveBrick(GameObject brick)
    {
        _activeBrick.Remove(brick);
        _timeManager.CheckToSpawnBoss();

    }
    public bool IsAllBrickDestroyed() => _activeBrick.Count <= 0;
    public List<GameObject> GetListOfBrick() => _brickPool;
    public List<GameObject> GetListOfActiveBrick() => _activeBrick;

}
