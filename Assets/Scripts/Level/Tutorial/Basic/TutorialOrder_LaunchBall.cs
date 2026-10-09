using UnityEngine;
using System.Collections;

public class TutorialOrder_LaunchBall : TutorialOrder
{
    protected override void GiveInstruction()
    {
        Debug.Log("Move mouse to aim. Press Left Mouse to launch ball");
    }

    protected override IEnumerator WaitForAction()
    {
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
    }
}
