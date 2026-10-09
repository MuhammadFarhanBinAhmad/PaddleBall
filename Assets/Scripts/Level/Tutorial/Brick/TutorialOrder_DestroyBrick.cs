using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class TutorialOrder_DestroyBrick : TutorialOrder
{
    [Header("Tutorial Brick")]
    [SerializeField] BrickGenerator _brickGenerator;
    [SerializeField] SOBrickFormation _tutorialFormation;
    [SerializeField] SO_BrickHealthStats _tutorialBrickStats;
    [SerializeField] SplineContainer _tutorialBrickPath;

    protected override void GiveInstruction()
    {
        Debug.Log("Destroy the brick!");
    }

    protected override IEnumerator WaitForAction()
    {
        if (_brickGenerator == null)
        {
            Debug.LogError("BrickGenerator is not assigned.");
            yield break;
        }

        if (_tutorialFormation == null)
        {
            Debug.LogError("Tutorial formation is not assigned.");
            yield break;
        }

        if (_tutorialBrickStats == null)
        {
            Debug.LogError("Tutorial brick stats are not assigned.");
            yield break;
        }

        if (_tutorialBrickPath == null)
        {
            Debug.LogError("Tutorial brick path is not assigned.");
            yield break;
        }

        _brickGenerator.SpawnTutorialFormation(
            _tutorialFormation,
            _tutorialBrickStats,
            _tutorialBrickPath
        );

        // Temporary until we add actual brick-destroy detection.
        yield return null;
    }
}