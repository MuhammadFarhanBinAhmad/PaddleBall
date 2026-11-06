using System.Collections;
using UnityEngine;

public class TutorialOrder_Movement : TutorialOrder
{


    protected override void GiveInstruction()
    {
        _tutorialHint.SetActive(true);
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
        _tutorialHint.SetActive(false);
        // Brief delay before completing the order.
        yield return new WaitForSecondsRealtime(_delayBeforeNextStep);
    }
}