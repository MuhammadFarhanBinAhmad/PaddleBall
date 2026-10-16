using System.Collections;
using UnityEngine;

public class TutorialOrder_Movement : TutorialOrder
{


    protected override void GiveInstruction()
    {
        Debug.Log("Use the A and D keys to move left and right.");
    }

    protected override IEnumerator WaitForAction()
    {
        bool pressedA = false;
        bool pressedD = false;

        while (!pressedA || !pressedD)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                pressedA = true;
                Debug.Log("A pressed!");
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                pressedD = true;
                Debug.Log("D pressed!");
            }

            yield return null;
        }

        // Both keys have been pressed.

        // Brief delay before completing the order.
        yield return new WaitForSecondsRealtime(_delayBeforeNextStep);
    }
}