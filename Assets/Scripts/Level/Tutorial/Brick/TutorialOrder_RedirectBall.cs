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
        bool holdingRightMouse = false;

        while (true)
        {
            // Step 1: RMB must be held
            if (Input.GetMouseButton(1))
            {
                holdingRightMouse = true;
            }
            else
            {
                holdingRightMouse = false;
            }

            // Step 2: LMB must be clicked while RMB is held
            if (holdingRightMouse && Input.GetMouseButtonDown(0))
            {
                yield break;
            }

            yield return null;
        }
    }
}
