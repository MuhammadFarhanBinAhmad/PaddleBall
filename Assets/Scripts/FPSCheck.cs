using TMPro;
using UnityEngine;

public class FPSCheck : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fpsText;

    [SerializeField] private float _updateInterval = 0.25f;

    private float _timer;
    private int _frameCount;

    void Update()
    {
        _timer += Time.unscaledDeltaTime;
        _frameCount++;

        if (_timer >= _updateInterval)
        {
            float fps = _frameCount / _timer;

            _fpsText.text = $"{Mathf.RoundToInt(fps)} FPS";

            _timer = 0f;
            _frameCount = 0;
        }
    }
}
