using UnityEngine;

public class PointOfFocusCutsceneEvent : BaseCutsceneEvent
{
    public Transform _pos;
    public GameObject _focusObject;

    public override void SetUpContent(SO_CutSceneEventContent content)
    {
        base.SetUpContent(content);
    }
    public override void EndEvent()
    {
        throw new System.NotImplementedException();
    }

    public override void ExecuteEvent()
    {
        throw new System.NotImplementedException();
    }
}
