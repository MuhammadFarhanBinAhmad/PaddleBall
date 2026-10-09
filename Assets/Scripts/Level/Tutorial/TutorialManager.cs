using System;
using System.Collections.Generic;
using UnityEngine;

public enum TUTORIAL_SECTION
{
    BASIC,
    BRICK,
    TOWER,
    CARDS_AND_SPELLS,
    BOOK
}
public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Section Order")]
    [SerializeField]
    private List<TutorialSection> _sectionOrder =
        new List<TutorialSection>();

    int _currentSectionIndex = -1;
    bool _tutorialActive;

    BrickGenerator _brickGenerator;
    CutSceneManager _cutSceneManager;

    public TUTORIAL_SECTION CurrentSection
    {
        get
        {
            if (!IsValidSectionIndex())
                throw new InvalidOperationException(
                    "Tutorial does not currently have a valid section."
                );

            return _sectionOrder[_currentSectionIndex].SectionType;
        }
    }

    public bool IsTutorialActive => _tutorialActive;

    public bool IsTutorialComplete =>
        !_tutorialActive &&
        _currentSectionIndex >= _sectionOrder.Count;

    public event Action<TUTORIAL_SECTION> OnSectionStarted;
    public event Action<TUTORIAL_SECTION> OnSectionCompleted;
    public event Action OnTutorialCompleted;

    private void Awake()
    {
        _brickGenerator = FindAnyObjectByType<BrickGenerator>();
        _cutSceneManager = FindAnyObjectByType<CutSceneManager>();
    }

    private void Start()
    {
        OnTutorialCompleted += _brickGenerator.StartGame;

        StartTutorial();
    }
    private void OnDestroy()
    {
        OnTutorialCompleted -= _brickGenerator.StartGame;
    }

    // --------------------------------------------------
    // Tutorial Start
    // --------------------------------------------------

    public void StartTutorial()
    {
        if (_sectionOrder == null || _sectionOrder.Count == 0)
        {
            Debug.LogWarning("Tutorial has no sections.");
            return;
        }

        _tutorialActive = true;
        _currentSectionIndex = 0;

        StartCurrentSection();
    }

    public void StartTutorialAt(TUTORIAL_SECTION section)
    {
        for (int i = 0; i < _sectionOrder.Count; i++)
        {
            if (_sectionOrder[i] != null &&
                _sectionOrder[i].SectionType == section)
            {
                _tutorialActive = true;
                _currentSectionIndex = i;

                StartCurrentSection();
                return;
            }
        }

        Debug.LogWarning(
            $"Tutorial section {section} is not in the current tutorial order."
        );
    }

    // --------------------------------------------------
    // Section Flow
    // --------------------------------------------------

    private void StartCurrentSection()
    {
        if (!IsValidSectionIndex())
        {
            CompleteTutorial();
            return;
        }

        TutorialSection section =
            _sectionOrder[_currentSectionIndex];

        if (section == null)
        {
            Debug.LogWarning(
                $"Tutorial section at index {_currentSectionIndex} is NULL."
            );

            _currentSectionIndex++;
            StartCurrentSection();
            return;
        }

        Debug.Log(
            $"Starting Tutorial Section: {section.SectionType}"
        );

        // Listen for this section finishing
        section.OnSectionCompleted -= HandleSectionCompleted;
        section.OnSectionCompleted += HandleSectionCompleted;

        OnSectionStarted?.Invoke(section.SectionType);

        // Actually start the Section
        section.StartSection();
    }

    private void HandleSectionCompleted(TutorialSection section)
    {
        section.OnSectionCompleted -= HandleSectionCompleted;

        CompleteCurrentSection();
    }

    public void CompleteCurrentSection()
    {
        if (!_tutorialActive)
            return;

        if (!IsValidSectionIndex())
            return;

        TUTORIAL_SECTION completedSection =
            _sectionOrder[_currentSectionIndex].SectionType;

        Debug.Log(
            $"Completed Tutorial Section: {completedSection}"
        );

        OnSectionCompleted?.Invoke(completedSection);

        _currentSectionIndex++;

        if (_currentSectionIndex >= _sectionOrder.Count)
        {
            CompleteTutorial();
            return;
        }

        StartCurrentSection();
    }

    // --------------------------------------------------
    // Section Navigation
    // --------------------------------------------------

    public void GoToSection(TUTORIAL_SECTION section)
    {
        for (int i = 0; i < _sectionOrder.Count; i++)
        {
            if (_sectionOrder[i] != null &&
                _sectionOrder[i].SectionType == section)
            {
                _currentSectionIndex = i;
                StartCurrentSection();
                return;
            }
        }

        Debug.LogWarning(
            $"Tutorial section {section} is not in the current tutorial order."
        );
    }

    public void GoToSectionIndex(int index)
    {
        if (index < 0 || index >= _sectionOrder.Count)
        {
            Debug.LogWarning(
                $"Tutorial section index {index} is invalid."
            );

            return;
        }

        _currentSectionIndex = index;

        StartCurrentSection();
    }

    // --------------------------------------------------
    // Tutorial Completion
    // --------------------------------------------------

    private void CompleteTutorial()
    {
        _tutorialActive = false;

        Debug.Log("Tutorial Completed!");

        OnTutorialCompleted?.Invoke();
    }

    // --------------------------------------------------
    // Helpers
    // --------------------------------------------------

    public int GetCurrentSectionIndex()
    {
        return _currentSectionIndex;
    }

    public int GetTotalSectionCount()
    {
        return _sectionOrder.Count;
    }

    public TutorialSection GetSection(int index)
    {
        if (index < 0 || index >= _sectionOrder.Count)
        {
            Debug.LogWarning(
                $"Tutorial section index {index} is invalid."
            );

            return null;
        }

        return _sectionOrder[index];
    }

    private bool IsValidSectionIndex()
    {
        return
            _sectionOrder != null &&
            _currentSectionIndex >= 0 &&
            _currentSectionIndex < _sectionOrder.Count;
    }
}