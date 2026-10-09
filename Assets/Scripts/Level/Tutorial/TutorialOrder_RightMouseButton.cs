using System.Collections;
using UnityEngine;

public class TutorialOrder_RightMouseButton : TutorialOrder
{
    protected override void GiveInstruction()
    {
        Debug.Log("Press the RIGHT MOUSE BUTTON to aim.");
    }

    protected override IEnumerator WaitForAction()
    {
        while (!Input.GetMouseButtonDown(1))
        {
            yield return null;
        }
    }
}
