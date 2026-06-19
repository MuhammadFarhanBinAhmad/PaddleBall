using System;
using UnityEngine;

public abstract class BaseCutsceneEvent : MonoBehaviour
{
    protected SO_CutSceneEventContent _content;
    protected CutSceneManager _cutSceneManager;

    public Action OnEventFinished;

    public virtual void SetUpContent(SO_CutSceneEventContent content)
    {
        _content = content;
    }

    public void SetCutSceneManager(CutSceneManager manager)
    {
        _cutSceneManager = manager;
    }

    public abstract void ExecuteEvent();
    public abstract void EndEvent();

    protected void NotifyFinished()
    {
        OnEventFinished?.Invoke();
    }
}
