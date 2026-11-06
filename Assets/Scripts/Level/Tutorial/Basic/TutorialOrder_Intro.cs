using System.Collections;
using UnityEngine;

public class TutorialOrder_Intro : TutorialOrder
{
    protected override void GiveInstruction()
    {
    }

    protected override IEnumerator WaitForAction()
    {
        yield return new WaitForSecondsRealtime(_delayBeforeNextStep);
    }

}
