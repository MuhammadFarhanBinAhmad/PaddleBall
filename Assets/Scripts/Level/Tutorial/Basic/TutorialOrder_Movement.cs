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
                _worldToCanvasPosition.ToggleFocusCircle(false);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                pressedD = true;
                Debug.Log("D pressed!");
                _worldToCanvasPosition.ToggleFocusCircle(false);
            }

            yield return null;
        }
    }

}
