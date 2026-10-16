using System.Collections;
using UnityEngine;

public class TutorialOrder_FinalAdvice : TutorialOrder
{
    protected override void GiveInstruction()
    {
    }

    protected override IEnumerator WaitForAction()
    {
        yield return new WaitForSeconds(_delayBeforeNextStep);
    }

}
