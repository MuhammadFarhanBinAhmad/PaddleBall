
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TutorialOrder_CreateFloor : TutorialOrder
{
    [Header("Tutorial Brick")]
    [SerializeField] BrickGenerator _brickGenerator;
    [SerializeField] SOBrickFormation _tutorialFormation;
    [SerializeField] SO_BrickHealthStats _tutorialBrickStats;

    [SerializeField]
    List<SplineContainer> _tutorialBrickPaths =
        new List<SplineContainer>();

    [Header("Floor Detection")]
    [SerializeField] TowerManager _towerManager;

    [Header("Spawn Settings")]
    [SerializeField] float _timerBeforeNextWave = 1f;
    [SerializeField] int _numberToSpawn = 1;

    [Header("Cleanup")]
    [SerializeField] float _cleanupDelay = 0.25f;

    // Bricks spawned by this tutorial order that remain alive.
    readonly List<BrickBar> _remainingBricks =
        new List<BrickBar>();

    bool _floorCreated;

    protected override void GiveInstruction()
    {
        Debug.Log(
            "Destroy bricks and collect enough Essence to create a floor!"
        );
    }

    protected override IEnumerator WaitForAction()
    {
        if (_brickGenerator == null ||
            _tutorialFormation == null ||
            _tutorialBrickStats == null ||
            _tutorialBrickPaths == null ||
            _tutorialBrickPaths.Count == 0 ||
            _towerManager == null)
        {
            Debug.LogError(
                "TutorialOrder_CreateFloor has missing references."
            );

            yield break;
        }

        _floorCreated = false;
        _remainingBricks.Clear();

        int startingTowerHeight = _towerManager._currentTowerHeight;

        // Listen for a newly created floor.
        _towerManager.OnHeightIncrease -= HandleFloorCreated;
        _towerManager.OnHeightIncrease += HandleFloorCreated;

        // Spawn and track tutorial bricks.
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

        // Wait for a newly created floor.
        while (!_floorCreated &&
               _towerManager != null &&
               _towerManager._currentTowerHeight <= startingTowerHeight)
        {
            yield return null;
        }

        StopListeningForFloor();

        Debug.Log("Tutorial floor successfully created!");

        // Clean up any tutorial bricks that remain alive.
        yield return StartCoroutine(CleanupRemainingBricks());

        yield return new WaitForSecondsRealtime(
            _delayBeforeNextStep
        );
    }

    private void RegisterSpawnedBrick(BrickBar brick)
    {
        if (brick == null || _remainingBricks.Contains(brick))
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

    private IEnumerator CleanupRemainingBricks()
    {
        // Work on a copy because destroying bricks modifies the list.
        List<BrickBar> bricksToClean = new List<BrickBar>(
            _remainingBricks
        );

        foreach (BrickBar brick in bricksToClean)
        {
            if (brick == null)
            {
                _remainingBricks.Remove(brick);
                continue;
            }

            // Unsubscribe before destruction to avoid duplicate removal.
            brick.OnBrickDestroyed -= HandleTutorialBrickDestroyed;
            _remainingBricks.Remove(brick);

            // Use the brick's existing death/pool logic.
            brick.HandleDeath();

            yield return new WaitForSecondsRealtime(_cleanupDelay);
        }

        _remainingBricks.Clear();

        Debug.Log("All remaining tutorial bricks have been cleaned up.");
    }

    private void HandleFloorCreated()
    {
        _floorCreated = true;
        Debug.Log("TowerManager detected a new floor.");
    }

    private void StopListeningForFloor()
    {
        if (_towerManager != null)
        {
            _towerManager.OnHeightIncrease -= HandleFloorCreated;
        }
    }

    private void OnDisable()
    {
        StopListeningForFloor();

        foreach (BrickBar brick in _remainingBricks)
        {
            if (brick != null)
            {
                brick.OnBrickDestroyed -= HandleTutorialBrickDestroyed;
            }
        }

        _remainingBricks.Clear();
    }
}
