using UnityEngine;

public class WorldToCanvasPosition : MonoBehaviour
{
    [SerializeField] Camera _worldCamera;
    [SerializeField] Canvas _canvas;
    [SerializeField] RectTransform _uiTarget;
    [SerializeField] internal RectTransform _textBox;

    public void UpdateFocusPosition(Transform _worldTarget)
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
            _uiTarget.parent as RectTransform;

        if (parentRect == null)
            return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPosition,
            canvasCamera,
            out Vector2 localPosition))
        {
            // 4. Position the UI element at that location.
            _uiTarget.localPosition = new Vector3(
                localPosition.x,
                localPosition.y,
                _uiTarget.localPosition.z
            );
        }
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
    public void ToggleFocusCircle(bool toggle) => _uiTarget.gameObject.SetActive(toggle);

}