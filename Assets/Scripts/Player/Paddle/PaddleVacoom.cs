using UnityEngine;
using FMOD.Studio;
using System.Collections.Generic;

public class PaddleVacoom : MonoBehaviour
{
    PaddleMovement _paddleMovement;
    PaddleHealth _paddleHealth;

    [Header("Vacuum Settings")]
    public float attractRadius = 3f;
    [Tooltip("Cone angle in degrees (total). 60 means ±30° from forward.")]
    public float coneAngle = 60f;
    public float _pushPullStrength = 10f;
    [Tooltip("Max force per FixedUpdate applied to balls by the vacuum.")]
    public float ballAttractForceCap = 5f;       // cap specifically for Ball
    public LayerMask collectibleLayer;           // set to the layer used by essences/balls
    [Header("Cone Visual")]
    [SerializeField] LineRenderer _coneLine;
    [SerializeField] int _coneSegments = 24;
    [SerializeField] Color _coneColor = new Color(1f, 1f, 1f, 0.35f);
    
    Camera _mainCamera;
    Vector2 _mouseForward;
    
    [SerializeField] int _maxSuctionTargets = 64;
    List<Collider2D> _suctionResults = new List<Collider2D>();
    ContactFilter2D _suctionFilter;

    bool _disableVacoom;

    EventInstance _paddleInhale;
    float inhalePower = 0;
    public float increaseSpeed = 1.5f;
    public float decreaseSpeed = 1f;
    private void Awake()
    {
        _paddleHealth = FindAnyObjectByType<PaddleHealth>();
        _paddleMovement = GetComponentInParent<PaddleMovement>();

        _mainCamera = Camera.main;

        _suctionResults.Capacity = _maxSuctionTargets;

        _suctionFilter = new ContactFilter2D();
        _suctionFilter.SetLayerMask(collectibleLayer);
        _suctionFilter.useTriggers = true;


    }
    private void Start()
    {
        _paddleInhale = AudioManager.Instance.CreateEventInstance(FmodEvent.Instance.sfx_onPaddleSucking);

        if (_coneLine != null)
        {
            _coneLine.loop = true;
            _coneLine.positionCount = _coneSegments + 2; // origin + arc points
            _coneLine.startColor = _coneColor;
            _coneLine.endColor = _coneColor;
        }

    }

    void Update()
    {
        if (_paddleHealth.IsPaddleDead())
            return;

        _mouseForward = GetMouseForward();

        IsSucking();

    }
    private void FixedUpdate()
    {
        if (_paddleHealth.IsPaddleDead())
            return;

        if (_disableVacoom)
            return;

        if (!Input.GetKey(KeyCode.Space))
            return;

        SuctionObject();
    }
    void UpdateConeVisual()
    {
        if (_coneLine == null) return;

        Vector2 forward = _mouseForward;
        Vector3 origin = transform.position;

        float halfAngle = coneAngle * 0.5f;
        float step = coneAngle / _coneSegments;

        _coneLine.positionCount = _coneSegments + 2;

        // first point = origin
        _coneLine.SetPosition(0, origin);

        for (int i = 0; i <= _coneSegments; i++)
        {
            float angle = -halfAngle + (step * i);
            Vector3 dir = Quaternion.Euler(0f, 0f, angle) * (Vector3)forward;
            Vector3 point = origin + dir * attractRadius;

            _coneLine.SetPosition(i + 1, point);
        }
    }
    void IsSucking()
    {

        if (_disableVacoom)
            return;

        bool attracting = Input.GetKey(KeyCode.Space);

        _paddleMovement.DisblePaddleMovement(attracting);
        _coneLine.enabled = attracting;

        if(attracting)
        {
            inhalePower += increaseSpeed * Time.deltaTime;


        }
        else
        {
            inhalePower -= increaseSpeed * Time.deltaTime;
        }
        inhalePower = Mathf.Clamp01(inhalePower);

        PlaySuctionAudio();
        UpdateConeVisual();

        //if (!attracting)
        //    return;
    }

    void SuctionObject()
    {

        Vector2 paddlePosition = transform.position;

        int hitCount = Physics2D.OverlapCircle(
            paddlePosition,
            attractRadius,
            _suctionFilter,
            _suctionResults
        );

        if (hitCount == 0)
            return;

        Vector2 forward = _mouseForward;

        float halfAngleRad = (coneAngle * 0.5f) * Mathf.Deg2Rad;
        float cosThreshold = Mathf.Cos(halfAngleRad);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D collider = _suctionResults[i];

            if (collider == null)
                continue;

            Vector2 toTarget =
                (Vector2)collider.transform.position - paddlePosition;

            float sqrDistance = toTarget.sqrMagnitude;

            if (sqrDistance <= 0.0001f)
                continue;

            Vector2 direction = toTarget.normalized;

            if (Vector2.Dot(forward, direction) < cosThreshold)
                continue;

            TowerEssence essence =
                collider.GetComponent<TowerEssence>();

            if (essence != null)
            {
                essence.StartAttraction(
                    transform,
                    _pushPullStrength,
                    attractRadius
                );

                essence.UpdateAttractionTarget(paddlePosition);
            }
        }
    }
    void PlaySuctionAudio()
    {
        _paddleInhale.setParameterByName("InhalePower", inhalePower);

        PLAYBACK_STATE state;
        _paddleInhale.getPlaybackState(out state);

        if (inhalePower > 0 && state != PLAYBACK_STATE.PLAYING)
        {
            _paddleInhale.start();
        }

        if (inhalePower <= 0 && state == PLAYBACK_STATE.PLAYING)
        {
            _paddleInhale.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
    /// <summary>
    /// Returns a normalized forward direction vector pointing from this object to the mouse world position.
    /// Falls back to transform.up if mouse world position is essentially the same as this object's position.
    /// </summary>
    Vector2 GetMouseForward()
    {
        if (_mainCamera == null)
            return transform.up.normalized;

        Vector3 mouseWorld3 =
            _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector2 toMouse =
            (Vector2)mouseWorld3 - (Vector2)transform.position;

        if (toMouse.sqrMagnitude < 0.0001f)
            return transform.up.normalized;

        return toMouse.normalized;
    }

    public void DisableVacoom(bool disable) => _disableVacoom = disable;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attractRadius); // optional radial guide

        // draw cone oriented toward mouse (or transform.up fallback)
        Vector3 origin = transform.position;
        Vector2 forward2 = GetGizmoForward();
        Vector3 forward3 = new Vector3(forward2.x, forward2.y, 0f);

        float halfAngle = coneAngle * 0.5f;

        // edge directions
        Vector3 leftDir = Quaternion.Euler(0f, 0f, -halfAngle) * forward3;
        Vector3 rightDir = Quaternion.Euler(0f, 0f, halfAngle) * forward3;

        Gizmos.color = new Color(1f, 1f, 1f, 0.6f);
        Gizmos.DrawLine(origin, origin + leftDir * attractRadius);
        Gizmos.DrawLine(origin, origin + rightDir * attractRadius);

        // draw arc
        int segments = 24;
        Vector3 prevPoint = origin + (Quaternion.Euler(0f, 0f, -halfAngle) * forward3) * attractRadius;
        for (int i = 1; i <= segments; i++)
        {
            float a = -halfAngle + (coneAngle * i / (float)segments);
            Vector3 nextPoint = origin + (Quaternion.Euler(0f, 0f, a) * forward3) * attractRadius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
    Vector2 GetGizmoForward()
    {
        if (Application.isPlaying)
        {
            // during play, GetMouseForward works fine
            return GetMouseForward();
        }


        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 mouseScreen = Input.mousePosition;
            Vector3 mouseWorld3 = cam.ScreenToWorldPoint(mouseScreen);
            Vector2 mouseWorld = new Vector2(mouseWorld3.x, mouseWorld3.y);
            Vector2 toMouse = mouseWorld - (Vector2)transform.position;
            if (toMouse.sqrMagnitude >= 0.0001f)
                return toMouse.normalized;
        }

        return transform.up.normalized;
    }


}
