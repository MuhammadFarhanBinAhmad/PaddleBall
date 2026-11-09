using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialOrder_ContinueNextDay : TutorialOrder
{
    [SerializeField] private Button _continueDay;

    private bool _buttonClicked;

    protected override void GiveInstruction()
    {

    }

    protected override IEnumerator WaitForAction()
    {
        _continueDay.gameObject.SetActive(true);
        _worldToCanvasPosition.ToggleFocus(true);

        _buttonClicked = false;

        _continueDay.onClick.AddListener(OnButtonClicked);

        // Wait until the player clicks the button.
        yield return new WaitUntil(() => _buttonClicked);
        _worldToCanvasPosition.ToggleFocus(false);
        // Remove the listener after clicking.
        _continueDay.onClick.RemoveListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        _buttonClicked = true;
    }
}
