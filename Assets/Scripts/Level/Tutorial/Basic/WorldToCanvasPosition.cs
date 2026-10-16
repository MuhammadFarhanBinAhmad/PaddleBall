using UnityEngine;

public class WorldToCanvasPosition : MonoBehaviour
{
    [SerializeField] Camera _worldCamera;
    [SerializeField] Canvas _canvas;
    [SerializeField] RectTransform [] _focusType;
    [SerializeField] RectTransform _focusUI;
    [SerializeField] internal RectTransform _textBox;

    public void UpdateFocusPosition(Transform _worldTarget,FOCUS_TYPE _type)
    {
        // 1. Convert world position to screen position.
        Vector3 screenPosition =
            _worldCamera.WorldToScreenPoint(_worldTarget.position);

        // Ignore targets behind the camera.
        if (screenPosition.z < 0f)
            return;

        // 2. Determine the camera used by the Canvas.
        Camera canvasCamera =
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : _canvas.worldCamera;

        // 3. Select the focus UI.
        switch (_type)
        {
            case FOCUS_TYPE.CIRCLE:
                _focusUI = _focusType[0];
                break;

            case FOCUS_TYPE.ARROW:
                _focusUI = _focusType[1];
                break;

            case FOCUS_TYPE.DOUBLE_ARROW:
                _focusUI = _focusType[2];
                break;
        }

        if (_focusUI == null)
            return;

        // 4. Convert screen position to the UI parent's local space.
        RectTransform parentRect =
            _focusUI.parent as RectTransform;

        if (parentRect == null)
            return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPosition,
            canvasCamera,
            out Vector2 localPosition))
        {
            _focusUI.localPosition = new Vector3(
                localPosition.x,
                localPosition.y,
                _focusUI.localPosition.z
            );
        }

        // 5. Calculate the target's rotation in screen space.
        Vector3 worldRight =
            _worldTarget.position + _worldTarget.right;

        Vector3 screenRight =
            _worldCamera.WorldToScreenPoint(worldRight);

        Vector2 screenDirection =
            screenRight - screenPosition;

        if (screenDirection.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(
                screenDirection.y,
                screenDirection.x
            ) * Mathf.Rad2Deg;

            // Apply the rotation to the UI focus element.
            _focusUI.rotation = Quaternion.Euler(
                0f,
                0f,
                angle
            );
        }

        ToggleFocus(true);
    }
    public void UpdateTextBoxPosition(Transform _worldTarget)
    {
        // 1. Convert world position to screen position.
        Vector3 screenPosition =
            _worldCamera.WorldToScreenPoint(_worldTarget.position);

        // Ignore targets behind the camera.
        if (screenPosition.z < 0f)
            return;

        // 2. Determine the camera used by the Canvas.
        Camera canvasCamera =
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : _canvas.worldCamera;

        // 3. Convert screen position to the UI parent's local space.
        RectTransform parentRect =
            _textBox.parent as RectTransform;

        if (parentRect == null)
            return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPosition,
            canvasCamera,
            out Vector2 localPosition))
        {
            // 4. Position the UI element at that location.
            _textBox.localPosition = new Vector3(
                localPosition.x,
                localPosition.y,
                _textBox.localPosition.z
            );
        }
    }
    public void ToggleFocus(bool toggle) => _focusUI.gameObject.SetActive(toggle);

}