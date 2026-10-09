using System.Collections;
using UnityEngine;

public class TutorialOrder_ManaExplanation : TutorialOrder
{
    protected override void GiveInstruction()
    {
        Debug.Log("The mana system is use to redirect the ball when its is full");

    }

    protected override IEnumerator WaitForAction()
    {

        yield return null;

    }
}
