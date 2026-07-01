using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    CutSceneManager _cutSceneManager;
    internal BaseBossBrick _baseBossBrick;

    public GameObject _bossPrefab;

    private void Awake()
    {
        _cutSceneManager = FindAnyObjectByType<CutSceneManager>();
    }
    public void SetBoss(GameObject boss) => _bossPrefab = boss;
    public void SpawnBoss()
    {
        GameObject boss = Instantiate(_bossPrefab, transform.position, Quaternion.identity);
        boss.transform.localScale = Vector3.zero;
        _baseBossBrick = boss.GetComponent<BaseBossBrick>();
        _cutSceneManager.FillCutSceneEvent(_baseBossBrick.GetStartCutSceneEvents());
        _cutSceneManager.SetCurrentBossGameObject(boss);
        _cutSceneManager.SetBossCutsceneToPlay(BOSSCUTSCENETOPLAY.INTRO);
        //Play first cutscene
        _cutSceneManager.StartCutScene();
    }
    public void OnBossDeath(BaseBossBrick _bbb)
    {
        _cutSceneManager.FillCutSceneEvent(_bbb.GetDefeatCutSceneEvents());
        _cutSceneManager.SetBossCutsceneToPlay(BOSSCUTSCENETOPLAY.DEFEATBOSS);
        _cutSceneManager.StartCutScene();
    }
    public void OnGameOver(BaseBossBrick _bbb)
    {
        _cutSceneManager.FillCutSceneEvent(_bbb.GetGameOverCutSceneEvents());
        _cutSceneManager.SetBossCutsceneToPlay(BOSSCUTSCENETOPLAY.GAMEOVER);
        _cutSceneManager.StartCutScene();
    }


}
