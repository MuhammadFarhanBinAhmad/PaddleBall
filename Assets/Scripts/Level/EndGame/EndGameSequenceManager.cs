
using System;
using System.Collections.Generic;
using UnityEngine;

public enum ENDGAME_STEP_TYPE
{
    INTROCUTSCENE,
    POPUP,
    ENDCUTSCNE
}

[Serializable]
public class EndGameStep
{
    public string _stepName;
    public ENDGAME_STEP_TYPE _stepType;

    [Header("Cutscene")]
    public List<SO_CutSceneEventContent> _cutsceneContent = new();

    [Header("Popup")]
    public EndGamePopup _popup;

}

public class EndGameSequenceManager : MonoBehaviour
{

    StatManager _statManager;
    CutSceneManager _cutsceneManager;

    [Header("Endgame Sequence")]
    [SerializeField] private List<EndGameStep> _steps = new();

    [Header("EndCutscene")]
    [SerializeField] List<int> _towerHeightGrade = new();
    [SerializeField] List<EndGameStep> _performanceCutscene = new();
    private CutSceneManager _cutSceneManager;

    private int _currentStepIndex = -1;
    private bool _isSequenceActive;

    public bool IsSequenceActive => _isSequenceActive;

    public event Action OnEndGameSequenceStarted;
    public event Action OnEndGameSequenceCompleted;

    [Header("Master")]
    public GameObject _master;
    private void Awake()
    {
        _cutSceneManager = FindAnyObjectByType<CutSceneManager>();
        _statManager = FindAnyObjectByType<StatManager>();
        _cutsceneManager = FindAnyObjectByType<CutSceneManager>();

    }
    private void Start()
    {
        _cutsceneManager.SetCurrentBossGameObject(_master);

        //StartEndGameSequence();

    }

    public void StartEndGameSequence()
    {
        if (_isSequenceActive)
            return;

        _isSequenceActive = true;
        _currentStepIndex = -1;


        SetResultCutScene();
        OnEndGameSequenceStarted?.Invoke();

        NextStep();
    }

    private void NextStep()
    {
        if (!_isSequenceActive)
            return;

        _currentStepIndex++;

        if (_currentStepIndex >= _steps.Count)
        {
            CompleteEndGameSequence();
            return;
        }

        EndGameStep currentStep = _steps[_currentStepIndex];

        switch (currentStep._stepType)
        {
            case ENDGAME_STEP_TYPE.INTROCUTSCENE:
                StartCutscene(currentStep);
                break;

            case ENDGAME_STEP_TYPE.POPUP:
                _cutsceneManager.SetCurrentBossGameObject(currentStep._popup.gameObject);
                StartPopup(currentStep);
                break;

            case ENDGAME_STEP_TYPE.ENDCUTSCNE:

                StartCutscene(currentStep);
                break;
        }
    }

    private void StartCutscene(EndGameStep step)
    {
        if (_cutSceneManager == null)
        {
            Debug.LogError("CutSceneManager not found!");
            return;
        }

        _cutSceneManager.PlayTutorialCutscene(
            step._cutsceneContent,
            NextStep
        );
    }

    private void StartPopup(EndGameStep step)
    {
        if (step._popup == null)
        {
            Debug.LogError($"Popup missing: {step._stepName}");
            return;
        }
        step._popup.gameObject.SetActive( true);
        step._popup.ShowPopup(NextStep);
    }

    private void CompleteEndGameSequence()
    {
        _isSequenceActive = false;

        Debug.Log("Endgame sequence completed!");

        OnEndGameSequenceCompleted?.Invoke();
    }
    public void SetResultCutScene()
    {
        int tower = _statManager.GetTowerHeight();

        // Default to the best grade if tower exceeds all thresholds.
        int grade = _towerHeightGrade.Count;

        for (int i = 0; i < _towerHeightGrade.Count; i++)
        {
            if (tower <= _towerHeightGrade[i])
            {
                grade = i;
                break;
            }
        }

        // Add the corresponding performance cutscene.
        if (grade >= 0 && grade < _performanceCutscene.Count)
        {
            _steps.Add(_performanceCutscene[grade]);
        }
        else
        {
            Debug.LogWarning($"No performance cutscene assigned for grade {grade}");
        }



    }
}
