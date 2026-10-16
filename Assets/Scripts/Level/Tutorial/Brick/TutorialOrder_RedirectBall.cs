using System.Collections;
using UnityEngine;

public class TutorialOrder_RedirectBall : TutorialOrder
{
    protected override void GiveInstruction()
    {
        Debug.Log("Hold Right Mouse to aim. Click Left Mouse to Shoot");
    }

    protected override IEnumerator WaitForAction()
    {
        _worldToCanvasPosition.ToggleFocus(false);
        while (true)
        {
            // RMB must be held while LMB is clicked.
            if (Input.GetMouseButton(1) &&
                Input.GetMouseButtonDown(0))
            {
                break;
            }

            yield return null;
        }

        // Delay before advancing to the next tutorial order.
        yield return new WaitForSecondsRealtime(_delayBeforeNextStep);
    }
}
