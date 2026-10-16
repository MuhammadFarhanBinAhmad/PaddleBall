using System.Collections;
using UnityEngine;

public class TutorialOrder_Health : TutorialOrder
{
    protected override void GiveInstruction()
    {
    }

    protected override IEnumerator WaitForAction()
    {
        _worldToCanvasPosition.ToggleFocus(false);
        yield return new WaitForSeconds(_delayBeforeNextStep);
    }
}
