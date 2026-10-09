using System.Collections;
using UnityEngine;

public class TutorialOrder_LeftMouseButton : TutorialOrder
{
    protected override void GiveInstruction()
    {
        Debug.Log("Press the LEFT MOUSE BUTTON to aim.");
    }

    protected override IEnumerator WaitForAction()
    {
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
    }
}
