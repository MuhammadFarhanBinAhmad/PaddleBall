using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    CutSceneManager _cutSceneManager;

    public List<GameObject> _bossPrefab;

    private void Awake()
    {
        _cutSceneManager = FindAnyObjectByType<CutSceneManager>();
    }

    public void SpawnBoss()
    {
        GameObject boss = Instantiate(_bossPrefab[0], transform.position, Quaternion.identity);
        BaseBossBrick _bbb = boss.GetComponent<BaseBossBrick>();
        _cutSceneManager.FillCutSceneEvent(_bbb.GetStartCutSceneEvents());
        _cutSceneManager.SetCurrentBossGameObject(boss);
        //Play first cutscene
        _cutSceneManager.StartCutScene();
    }
    public void OnBossDeath(BaseBossBrick _bbb)
    {
        _cutSceneManager.FillCutSceneEvent(_bbb.GetDefeatCutSceneEvents());
        _cutSceneManager.StartCutScene();
    }


}
