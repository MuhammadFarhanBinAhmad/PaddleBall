using UnityEngine;
using System.Collections;

public class TutorialOrder_LaunchBall : TutorialOrder
{
    protected override void GiveInstruction()
    {
        _tutorialHint.SetActive(true);
    }

    protected override IEnumerator WaitForAction()
    {
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
        _tutorialHint.SetActive(false);
        // Wait briefly after the player clicks.
        yield return new WaitForSecondsRealtime(_delayBeforeNextStep);
    }
}
