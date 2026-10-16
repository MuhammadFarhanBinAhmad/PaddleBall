using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TutorialOrder_DestroyBrick : TutorialOrder
{
    [Header("Tutorial Brick")]
    [SerializeField] BrickGenerator _brickGenerator;
    [SerializeField] SOBrickFormation _tutorialFormation;
    [SerializeField] SO_BrickHealthStats _tutorialBrickStats;

    [SerializeField]
    List<SplineContainer> _tutorialBrickPaths =
        new List<SplineContainer>();

    [Header("Spawn Settings")]
    [SerializeField] float _timerBeforeNextWave = 1f;
    [SerializeField] int _numberToSpawn = 1;

    // Bricks spawned by this tutorial order that remain alive.
    readonly List<BrickBar> _remainingBricks =
        new List<BrickBar>();

    protected override void GiveInstruction()
    {
        Debug.Log("Destroy the bricks!");
    }

    protected override IEnumerator WaitForAction()
    {
        if (_brickGenerator == null ||
            _tutorialFormation == null ||
            _tutorialBrickStats == null ||
            _tutorialBrickPaths == null ||
            _tutorialBrickPaths.Count == 0)
        {
            Debug.LogError(
                "TutorialOrder_DestroyBrick has missing references."
            );
            yield break;
        }

        _remainingBricks.Clear();

        // Spawn each formation and wait for its spawning coroutine
        // to finish before scheduling the next formation.
        for (int i = 0; i < _numberToSpawn; i++)
        {
            yield return _brickGenerator.SpawnTutorialFormationRoutine(
                _tutorialFormation,
                _tutorialBrickStats,
                _tutorialBrickPaths,
                RegisterSpawnedBrick
            );

            if (i < _numberToSpawn - 1)
            {
                yield return new WaitForSeconds(
                    _timerBeforeNextWave
                );
            }
        }

        // All formations have finished spawning.
        // Wait until every tracked brick has been destroyed.
        while (_remainingBricks.Count > 0)
        {
            yield return new WaitForSeconds(_delayBeforeNextStep);
        }

        Debug.Log("All tutorial bricks destroyed.");
    }

    private void RegisterSpawnedBrick(BrickBar brick)
    {
        if (brick == null)
            return;

        if (_remainingBricks.Contains(brick))
            return;

        _remainingBricks.Add(brick);
        brick.OnBrickDestroyed += HandleTutorialBrickDestroyed;

        Debug.Log(
            $"Tutorial brick registered. Remaining: {_remainingBricks.Count}"
        );
    }

    private void HandleTutorialBrickDestroyed(BrickBar brick)
    {
        if (brick == null)
            return;

        brick.OnBrickDestroyed -= HandleTutorialBrickDestroyed;

        if (_remainingBricks.Remove(brick))
        {
            Debug.Log(
                $"Tutorial brick removed. Remaining: {_remainingBricks.Count}"
            );
        }
    }

    private void OnDisable()
    {
        // Clean up event subscriptions if the order is interrupted.
        foreach (BrickBar brick in _remainingBricks)
        {
            if (brick != null)
                brick.OnBrickDestroyed -= HandleTutorialBrickDestroyed;
        }

        _remainingBricks.Clear();
    }
}