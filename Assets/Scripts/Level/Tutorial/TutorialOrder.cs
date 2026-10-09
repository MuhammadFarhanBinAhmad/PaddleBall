using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class TutorialOrder : MonoBehaviour
{
    public bool IsCompleted { get; private set; }

    [Header("Focus Target")]
    [SerializeField] internal WorldToCanvasPosition _worldToCanvasPosition;
    [SerializeField] Transform _target;
    [Header("Text Box")]
    [SerializeField] Transform _textBoxPos;

    public bool _showFocus;
    public bool _moveTextBoxPosition;

    public event Action<TutorialOrder> OnOrderCompleted;
    public List <SO_CutSceneEventContent> _cutsceneContent = new List<SO_CutSceneEventContent>();
    /// <summary>
    /// Starts the tutorial order.
    /// Handles the common instruction -> action flow.
    /// </summary>
    public void StartOrder()
    {
        IsCompleted = false;
        StartCoroutine(RunOrder());
    }
    public void ShowFocusArea()
    {
        _worldToCanvasPosition.UpdateFocusPosition(_target);
    }
    private IEnumerator RunOrder()
    {
        _worldToCanvasPosition.UpdateTextBoxPosition(_textBoxPos);
        // 1. Give instruction
        GiveInstruction();

        // 2. Wait for player action
        yield return WaitForAction();

        // 3. Complete order
        CompleteOrder();
    }

    /// <summary>
    /// Child class defines what instruction the player receives.
    /// </summary>
    protected abstract void GiveInstruction();

    /// <summary>
    /// Child class defines what the player must do.
    /// The coroutine finishes when the required action is performed.
    /// </summary>
    protected abstract IEnumerator WaitForAction();

    protected virtual void CompleteOrder()
    {
        IsCompleted = true;

        Debug.Log(
            $"Tutorial Order Completed: {GetType().Name}"
        );

        OnOrderCompleted?.Invoke(this);
    }
}