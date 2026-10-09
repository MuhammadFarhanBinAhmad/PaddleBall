using UnityEngine;
using UnityEngine.Splines;

public class Test : MonoBehaviour
{
    [Header("Tutorial Brick")]
    [SerializeField] BrickGenerator _brickGenerator;
    [SerializeField] SOBrickFormation _tutorialFormation;
    [SerializeField] SO_BrickHealthStats _tutorialBrickStats;
    [SerializeField] SplineContainer _tutorialBrickPath;


    private void Start()
    {
        print("hit");

        if (_brickGenerator == null)
        {
            Debug.LogError("BrickGenerator is not assigned.");
        }

        if (_tutorialFormation == null)
        {
            Debug.LogError("Tutorial Formation is not assigned.");
        }

        _brickGenerator.SpawnTutorialFormation(
            _tutorialFormation,
            _tutorialBrickStats,
            _tutorialBrickPath
        );
    }
}
