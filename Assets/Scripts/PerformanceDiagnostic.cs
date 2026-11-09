
using UnityEngine;

public class PerformanceDiagnostic : MonoBehaviour
{
    private int _frameCount;
    private int _fixedUpdateCount;
    private float _elapsed;

    private void FixedUpdate()
    {
        _fixedUpdateCount++;
    }

    private void Update()
    {
        _frameCount++;
        _elapsed += Time.unscaledDeltaTime;

        if (_elapsed < 5f)
            return;

        float fps = _frameCount / _elapsed;
        float physicsUpdates = _fixedUpdateCount / _elapsed;

        Debug.Log(
            $"FPS: {fps:F1} | " +
            $"FixedUpdates/sec: {physicsUpdates:F1} | " +
            $"FixedDeltaTime: {Time.fixedDeltaTime:F6} | " +
            $"TimeScale: {Time.timeScale:F3}"
        );

        _frameCount = 0;
        _fixedUpdateCount = 0;
        _elapsed = 0f;
    }
}
