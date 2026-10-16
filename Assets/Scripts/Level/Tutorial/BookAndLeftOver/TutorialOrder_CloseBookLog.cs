
using System.Collections;
using UnityEngine;

public class TutorialOrder_CloseBookLog : TutorialOrder
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Book Log")]
    [SerializeField] BookOverlayUI _bookOverlayUI;

    bool _bookLogOpened;

    protected override void GiveInstruction()
    {
        Debug.Log("Open the Book Log!");
    }

    protected override IEnumerator WaitForAction()
    {
        if (_bookOverlayUI == null)
        {
            Debug.LogError(
                "TutorialOrder_OpenBookLog: BookOverlayUI reference is missing."
            );

            yield break;
        }

        _bookLogOpened = false;

        // Listen for the Book Log button click.
        _bookOverlayUI.OnBookLogClosed += HandleBookLogOpened;

        // Wait until the player clicks the button.
        while (!_bookLogOpened)
        {
            yield return null;
        }

        // Stop listening once the action is complete.
        _bookOverlayUI.OnBookLogClosed -= HandleBookLogOpened;

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
            _bookOverlayUI.OnBookLogClosed -= HandleBookLogOpened;
        }
    }
}
