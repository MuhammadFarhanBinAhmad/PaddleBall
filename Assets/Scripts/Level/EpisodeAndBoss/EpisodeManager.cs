using System;
using System.Collections.Generic;
using UnityEngine;
public class EpisodeManager : MonoBehaviour
{
    BossManager _bossManager;

    [SerializeField] SO_EpisodeDetails _firstEpisode;
    [SerializeField] List<SO_EpisodeDetails> _episodeList = new List<SO_EpisodeDetails>();
    [SerializeField] Transform _levelSpawnPos;
    [SerializeField] GameObject _currentLevel;

    public Action<int> OnStartEpisode;
    bool _firstEP = true;

    private void Awake()
    {
        _bossManager = FindAnyObjectByType<BossManager>();
    }
    private void Start()
    {
        OnStartEpisode += SetBoss;
        OnStartEpisode += SetLevel;
        OnStartEpisode += RemoveEpisode;

        SetFirstEpisode();
    }
    private void OnDestroy()
    {
        OnStartEpisode -= SetBoss;
        OnStartEpisode -= SetLevel;
        OnStartEpisode -= RemoveEpisode;
    }

    void SetFirstEpisode()
    {
        _bossManager.SetBoss(_firstEpisode._bossPrefab);
        GameObject level = Instantiate(_firstEpisode._levelLayout, _levelSpawnPos.position, Quaternion.identity);
        _currentLevel = level;
    }

    public int GetTotalEpisode() => _episodeList.Count;
    public void SetBoss(int index)
    {
        _bossManager.SetBoss(_episodeList[index]._bossPrefab);
    }
    public void RemoveEpisode(int index)
    {
        if (_firstEP)
        {
            _firstEP = false;
            return;
        }
        _episodeList.RemoveAt(index);
    }
    public void SetLevel(int index)
    {
        GameObject level = Instantiate(_episodeList[index]._levelLayout, _levelSpawnPos.position,Quaternion.identity);
        GameObject prevlevel = _currentLevel;
        _currentLevel = level;
        Destroy(prevlevel);
    }
}
