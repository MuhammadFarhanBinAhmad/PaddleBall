using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class B_ShieldAbility : MonoBehaviour
{
    SO_BrickAbilityStats _stats;

    bool _activateStaticShield;
    bool _activateMovingShield;

    [Header("Static Shield")]
    public List<GameObject> _staticShield = new List<GameObject>();
    SHIELD_SIDE _ss;

    [Header("Moving Shield")]
    [SerializeField] SplineContainer _spline;
    public List<GameObject> _movingShield = new List<GameObject>();
    // Normalized position of each shield along the spline
    List<float> _shieldPositions = new List<float>();

    float _movementSpeed;
    float _upTime;
    float _rechargeTime;

    public void SetStats(SO_BrickAbilityStats stats)
    {
        _stats = stats;

        _ss = _stats._shieldSide;
        _upTime = _stats._upTime;
        _rechargeTime = _stats._rechargeTime;
        _movementSpeed = _stats._movementSpeed;

        _activateMovingShield = _stats._activateMovingShield;
        _activateStaticShield = _stats._activateStaticShield;

        // Static shields
        if (_activateStaticShield)
            ActivateShield();
        else
            SetShieldListActive(_staticShield, false);

        // Moving shields
        SetShieldListActive(_movingShield, _activateMovingShield);

        if (_activateMovingShield)
            InitializeShieldPositions();
    }

    public void ActivateShield()
    {
        // Disable all static shields first
        SetShieldListActive(_staticShield, false);

        if ((_ss & SHIELD_SIDE.FRONT) != 0 && _staticShield.Count > 0)
            _staticShield[0].SetActive(true);

        if ((_ss & SHIELD_SIDE.LEFT) != 0 && _staticShield.Count > 1)
            _staticShield[1].SetActive(true);

        if ((_ss & SHIELD_SIDE.RIGHT) != 0 && _staticShield.Count > 2)
            _staticShield[2].SetActive(true);

        if ((_ss & SHIELD_SIDE.BACK) != 0 && _staticShield.Count > 3)
            _staticShield[3].SetActive(true);
    }

    private void InitializeShieldPositions()
    {
        _shieldPositions.Clear();

        if (_spline == null || _spline.Spline == null)
            return;

        int count = _movingShield.Count;

        if (count == 0 || _spline.Spline.Count < 2)
            return;

        for (int i = 0; i < count; i++)
        {
            // Evenly distribute shields around the spline
            float t = (float)i / count;

            _shieldPositions.Add(t);

            UpdateShieldPosition(i);
        }
    }

    private void Update()
    {
        if (!_activateMovingShield || _spline == null)
            return;

        if (_shieldPositions.Count != _movingShield.Count)
            return;

        for (int i = 0; i < _movingShield.Count; i++)
        {
            if (_movingShield[i] == null)
                continue;

            // Move along the spline, looping from 1 back to 0
            _shieldPositions[i] =
                (_shieldPositions[i] + _movementSpeed * Time.deltaTime) % 1f;

            UpdateShieldPosition(i);
        }
    }

    private void UpdateShieldPosition(int index)
    {
        if (_spline == null || index >= _movingShield.Count)
            return;

        if (_movingShield[index] == null)
            return;

        float t = _shieldPositions[index];

        // Evaluate position on the container's spline
        float3 position = SplineUtility.EvaluatePosition(
            _spline.Spline,
            t
        );

        // Convert spline-local position into world position
        Vector3 worldPosition =
            _spline.transform.TransformPoint((Vector3)position);

        _movingShield[index].transform.position = worldPosition;
    }

    private void SetShieldListActive(
        List<GameObject> shields,
        bool isActive)
    {
        foreach (GameObject shield in shields)
        {
            if (shield != null)
                shield.SetActive(isActive);
        }
    }
}