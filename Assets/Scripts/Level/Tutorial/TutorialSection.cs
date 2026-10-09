using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSection : MonoBehaviour
{
    CutSceneManager _cutSceneManager;

    [Header("Section")]
    [SerializeField] TUTORIAL_SECTION _sectionType;

    [Header("Orders")]
    [SerializeField]
    internal List<TutorialOrder> _orders =
        new List<TutorialOrder>();

    internal int _currentOrderIndex = -1;
    bool _sectionActive;

    public TUTORIAL_SECTION SectionType => _sectionType;
    public bool IsSectionActive => _sectionActive;

    public event Action<TutorialSection> OnSectionCompleted;

    // --------------------------------------------------
    // Section Flow
    // --------------------------------------------------
    private void Awake()
    {
        _cutSceneManager = FindAnyObjectByType<CutSceneManager>();
    }
    public void StartSection()
    {
        if (_orders == null || _orders.Count == 0)
        {

            CompleteSection();
            return;
        }

        _sectionActive = true;
        _currentOrderIndex = 0;

        StartCurrentOrder();
    }

    private void StartCurrentOrder()
    {
        if (!_sectionActive)
            return;

        if (_currentOrderIndex < 0 ||
            _currentOrderIndex >= _orders.Count)
        {
            CompleteSection();
            return;
        }

        TutorialOrder order = _orders[_currentOrderIndex];

        if (order == null)
        {

            MoveToNextOrder();
            return;
        }

        order.OnOrderCompleted += HandleOrderCompleted;

        // --------------------------------------------
        // Has cutscene?
        // --------------------------------------------

        if (_cutSceneManager != null &&
            order._cutsceneContent != null &&
            order._cutsceneContent.Count > 0)
        {
            if (order._showFocus)
            {
                order.ShowFocusArea();
            }

            _cutSceneManager.PlayTutorialCutscene(
            order._cutsceneContent,
            () =>
            {
                StartOrderAfterCutscene(order);
            }

        );

        }
        else
        {
            // No cutscene ¨ start immediately
            StartOrderAfterCutscene(order);
        }
    }
    private void StartOrderAfterCutscene(TutorialOrder order)
    {
        if (!_sectionActive)
            return;

        order.StartOrder();
    }
    private void HandleOrderCompleted(TutorialOrder order)
    {
        order.OnOrderCompleted -= HandleOrderCompleted;

        MoveToNextOrder();
    }

    private void MoveToNextOrder()
    {
        _currentOrderIndex++;

        if (_currentOrderIndex >= _orders.Count)
        {
            CompleteSection();
            return;
        }

        StartCurrentOrder();
    }

    // --------------------------------------------------
    // Section Completion
    // --------------------------------------------------

    private void CompleteSection()
    {
        if (!_sectionActive)
            return;

        _sectionActive = false;

        Debug.Log(
            $"Tutorial Section Completed: {_sectionType}"
        );

        OnSectionCompleted?.Invoke(this);
    }

    // --------------------------------------------------
    // Optional Helpers
    // --------------------------------------------------

    public int GetCurrentOrderIndex()
    {
        return _currentOrderIndex;
    }

    public int GetOrderCount()
    {
        return _orders.Count;
    }

    public TutorialOrder GetCurrentOrder()
    {
        if (_currentOrderIndex < 0 ||
            _currentOrderIndex >= _orders.Count)
        {
            return null;
        }

        return _orders[_currentOrderIndex];
    }
}