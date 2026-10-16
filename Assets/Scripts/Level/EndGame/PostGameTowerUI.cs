
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PostGameTowerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StatManager _statManager;
    [SerializeField] private TMP_Text _towerHeightText;

    [Header("Tower Visual")]
    [SerializeField] private RectTransform _towerContainer;
    [SerializeField] private RectTransform _floorPrefab;
    [SerializeField] private float _floorHeight = 25f;

    [Header("Height Settings")]
    [SerializeField] private int _metersPerFloor = 1;
    [SerializeField] private int _maxVisualFloors = 30;

    [Header("Animation Settings")]
    [SerializeField] private float _buildDuration = 3f;
    [SerializeField] private float _floorPopDuration = 0.15f;
    [SerializeField]
    private AnimationCurve _heightCurve =
        AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Tower Scroll View")]
    [SerializeField] private ScrollRect _towerScrollRect;
    [SerializeField] private RectTransform _scrollContent;
    [SerializeField] private float _scrollPadding = 50f;
    [SerializeField] private float _scrollSmoothSpeed = 8f;

    private Coroutine _buildCoroutine;
    private bool _isBuilding;

    public bool IsBuilding => _isBuilding;

    public event Action OnTowerBuildCompleted;

    private void Awake()
    {
        if (_statManager == null)
            _statManager = FindAnyObjectByType<StatManager>();
    }
    private void LateUpdate()
    {
        if (!_isBuilding || _towerScrollRect == null)
            return;

        float targetScrollPosition = 0;

        _towerScrollRect.horizontalNormalizedPosition = Mathf.Lerp(
            _towerScrollRect.horizontalNormalizedPosition,
            targetScrollPosition,
            Time.unscaledDeltaTime * _scrollSmoothSpeed
        );
    }
    public void StartTowerAnimation()
    {
        if (_statManager == null ||
    _towerHeightText == null ||
    _towerContainer == null ||
    _floorPrefab == null ||
    _towerScrollRect == null ||
    _scrollContent == null)
        {
            Debug.LogError("PostGameTowerUI: Missing references!");
            return;
        }

        if (_buildCoroutine != null)
        {
            StopCoroutine(_buildCoroutine);
            _buildCoroutine = null;
        }

        // Clear previously generated floors.
        ClearTower();

        // Reset the content height.
        _scrollContent.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            _towerScrollRect.viewport.rect.height
        );

        // Reset scrolling to the bottom.
        _towerScrollRect.StopMovement();
        _towerScrollRect.horizontalNormalizedPosition = 0f;

        _towerHeightText.text = "Tower Height: 0m";
        _isBuilding = true;

        _buildCoroutine = StartCoroutine(BuildTowerRoutine());
    }

    private IEnumerator BuildTowerRoutine()
    {
        int targetHeight = Mathf.Max(0, _statManager._towerHeight);

        int targetFloors = Mathf.CeilToInt(
            targetHeight / (float)Mathf.Max(1, _metersPerFloor)
        );

        // Limit the number of UI floor objects.
        int visualFloors = Mathf.Min(
            targetFloors,
            Mathf.Max(0, _maxVisualFloors)
        );

        int createdFloors = 0;
        float elapsed = 0f;
        float duration = Mathf.Max(0f, _buildDuration);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float progress = Mathf.Clamp01(_heightCurve.Evaluate(t));

            int currentHeight = Mathf.RoundToInt(
                Mathf.Lerp(0, targetHeight, progress)
            );

            _towerHeightText.text =
                $"Tower Height: {currentHeight:N0}m";

            // Keep tower construction synchronised with height.
            int expectedFloors = Mathf.FloorToInt(
                visualFloors * progress
            );

            while (createdFloors < expectedFloors)
            {
                CreateFloor(createdFloors);
                createdFloors++;
            }

            yield return null;
        }

        // Ensure the final height and all floors are displayed.
        while (createdFloors < visualFloors)
        {
            CreateFloor(createdFloors);
            createdFloors++;
        }

        _towerHeightText.text =
            $"Tower Height: {targetHeight:N0}m";

        //Canvas.ForceUpdateCanvases();

        _isBuilding = false;
        _buildCoroutine = null;

        OnTowerBuildCompleted?.Invoke();
    }

    private void CreateFloor(int floorIndex)
    {
        RectTransform floor = Instantiate(
        _floorPrefab,
        _towerContainer
    );

        floor.gameObject.SetActive(true);

        floor.anchoredPosition = new Vector2(
            0f,
            floorIndex * _floorHeight
        );

        floor.localScale = Vector3.one;

        float towerHeight = (floorIndex + 1) * _floorHeight;

        float contentHeight = Mathf.Max(
            _towerScrollRect.viewport.rect.height,
            towerHeight + _scrollPadding
        );

        _scrollContent.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            contentHeight
        );

        // Make sure the scroll view uses the updated content layout.
        Canvas.ForceUpdateCanvases();

        StartCoroutine(AnimateFloorPop(floor));
    }

    private IEnumerator AnimateFloorPop(RectTransform floor)
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0f, _floorPopDuration);

        Vector3 finalScale = floor.localScale;
        floor.localScale = Vector3.zero;

        if (duration <= 0f)
        {
            floor.localScale = finalScale;
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            float scale = Mathf.SmoothStep(0f, 1f, t);

            floor.localScale = finalScale * scale;

            yield return null;
        }

        floor.localScale = finalScale;
    }

    public void SkipTowerAnimation()
    {
        if (!_isBuilding)
            return;

        if (_buildCoroutine != null)
        {
            StopCoroutine(_buildCoroutine);
            _buildCoroutine = null;
        }

        int targetHeight = Mathf.Max(0, _statManager._towerHeight);

        int targetFloors = Mathf.CeilToInt(
            targetHeight / (float)Mathf.Max(1, _metersPerFloor)
        );

        int visualFloors = Mathf.Min(
            targetFloors,
            Mathf.Max(0, _maxVisualFloors)
        );

        for (int i = 0; i < visualFloors; i++)
        {
            RectTransform floor = Instantiate(
                _floorPrefab,
                _towerContainer
            );

            floor.gameObject.SetActive(true);
            floor.anchoredPosition = new Vector2(
                0f,
                i * _floorHeight
            );

            floor.localScale = Vector3.one;
        }

        _towerHeightText.text =
            $"Tower Height: {targetHeight:N0}m";

        _isBuilding = false;

        OnTowerBuildCompleted?.Invoke();
    }

    private void ClearTower()
    {
        foreach (Transform child in _towerContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnDisable()
    {
        if (_buildCoroutine != null)
        {
            StopCoroutine(_buildCoroutine);
            _buildCoroutine = null;
        }

        _isBuilding = false;
    }
}
