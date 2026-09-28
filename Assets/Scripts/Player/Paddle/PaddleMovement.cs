using System.Collections;
using UnityEditor;
using UnityEngine;

public class PaddleMovement : MonoBehaviour
{
    PolygonCollider2D _polygonCollider;

    [Header("Speed")]
    [SerializeField] float _originalSpeed;
    [SerializeField] float _cursedSpeed;
    float _speed;


    public float _maxXPos;
    [SerializeField]bool _disablePaddleMovement;

    private void Awake()
    {
        _polygonCollider = GetComponent<PolygonCollider2D>();
    }
    private void Start()
    {
        _speed = _originalSpeed;
    }

    void Update()
    {
        if (_disablePaddleMovement)
            return;

        if (Input.GetKey(KeyCode.A) && transform.position.x > -_maxXPos)
        {
            transform.position += Vector3.left * _speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D) && transform.position.x < _maxXPos)
        {
            transform.position += Vector3.right * _speed * Time.deltaTime;
        }

        //float mouseX = Input.GetAxis("Mouse X");

        //if ((mouseX > 0 && transform.position.x < _maxXPos) ||
        //    (mouseX < 0 && transform.position.x > -_maxXPos))
        //{
        //    transform.position += Vector3.right * mouseX * _speed * Time.deltaTime;
        //}
    }

    public void DisblePaddleMovement(bool disable)
    {
        _disablePaddleMovement = disable;
    }
    public void DisblePaddleCollider(bool disable)
    {
        _polygonCollider.enabled = !disable;
    }

    public void IsCursed(float duration)
    {
        StartCoroutine(SlowSpeed(duration));
    }

    IEnumerator SlowSpeed(float duration)
    {
        _speed = _cursedSpeed;
        yield return new WaitForSeconds(duration);
        _speed = _originalSpeed;
    }


}
