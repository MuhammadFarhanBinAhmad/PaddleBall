using System.Collections;
using UnityEngine;

public class TutorialOrder_EssenceToBrick : TutorialOrder
{
    protected override void GiveInstruction()
    {
    }

    protected override IEnumerator WaitForAction()
    {
        _worldToCanvasPosition.ToggleFocus(false);
        return null;
    }
}
