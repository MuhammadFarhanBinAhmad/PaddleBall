using System;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    int _controlLockCount;

    public bool ControlsEnabled =>
        _controlLockCount <= 0;

    public event Action OnControlsDisabled;
    public event Action OnControlsEnabled;

    public void DisableControls()
    {
        bool wasEnabled = ControlsEnabled;

        _controlLockCount++;

        if (wasEnabled)
        {
            OnControlsDisabled?.Invoke();
        }
    }

    public void EnableControls()
    {
        if (_controlLockCount <= 0)
            return;

        _controlLockCount--;

        if (_controlLockCount == 0)
        {
            OnControlsEnabled?.Invoke();
        }
    }

    public void ForceEnableControls()
    {
        _controlLockCount = 0;
        OnControlsEnabled?.Invoke();
    }
}