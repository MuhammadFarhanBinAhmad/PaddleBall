
using System.Collections;
using UnityEngine;

public class TutorialOrder_OpenBookLog : TutorialOrder
{
    [Header("Book Log")]
    [SerializeField] BookOverlayUI _bookOverlayUI;

    bool _bookLogOpened;

    protected override void GiveInstruction()
    {
        Debug.Log("Open the Book Log!");
    }

    protected override IEnumerator WaitForAction()
    {
        _worldToCanvasPosition.ToggleFocus(false);
        if (_bookOverlayUI == null)
        {
            Debug.LogError(
                "TutorialOrder_OpenBookLog: BookOverlayUI reference is missing."
            );

            yield break;
        }

        _bookLogOpened = false;

        // Listen for the Book Log button click.
        _bookOverlayUI.OnBookLogOpened += HandleBookLogOpened;

        // Wait until the player clicks the button.
        while (!_bookLogOpened)
        {
            yield return null;
        }

        // Stop listening once the action is complete.
        _bookOverlayUI.OnBookLogOpened -= HandleBookLogOpened;

        yield return new WaitForSecondsRealtime(_delayBeforeNextStep);
    }

    private void HandleBookLogOpened()
    {
        _bookLogOpened = true;
        Debug.Log("Book Log button clicked!");
    }

    private void OnDisable()
    {
        // Clean up if the tutorial order is interrupted.
        if (_bookOverlayUI != null)
        {
            _bookOverlayUI.OnBookLogOpened -= HandleBookLogOpened;
        }
    }
}
