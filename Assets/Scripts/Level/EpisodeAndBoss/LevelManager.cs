using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class LevelManager : MonoBehaviour
{
    [SerializeField] List<GameObject> _walls = new List<GameObject>();
    [SerializeField] List<SplineContainer> _waypoint = new List<SplineContainer>();
    [SerializeField] List<SOBrickFormation> _formation = new List<SOBrickFormation>();

    GlobalFeedbackManager _feedbackManager;
    BrickGenerator _brickGenerator;

    private void Awake()
    {
        _feedbackManager = FindAnyObjectByType<GlobalFeedbackManager>();
        _brickGenerator = FindAnyObjectByType<BrickGenerator>();
        _brickGenerator.SetLevelManager(this);
        _brickGenerator.SetBrickPath(_waypoint);
        _brickGenerator.SetFormation(_formation);
    }

    private void Start()
    {
        SetWallForLevel();
    }
    public void SetWallForLevel() => _feedbackManager.SetWall(_walls);
    public void SetFormationForLevel() => _brickGenerator.SetFormation(_formation);

}
