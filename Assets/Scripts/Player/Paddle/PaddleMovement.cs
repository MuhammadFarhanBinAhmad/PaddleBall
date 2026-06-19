using UnityEditor;
using UnityEngine;

public class PaddleMovement : MonoBehaviour
{
    PolygonCollider2D _polygonCollider;

    public float _speed;
    public float _maxXPos;
    [SerializeField]bool _disablePaddleMovement;

    private void Awake()
    {
        _polygonCollider = GetComponent<PolygonCollider2D>();
    }

    void Update()
    {
        if (_disablePaddleMovement)
            return;

        float mouseX = Input.GetAxis("Mouse X");

        if ((mouseX > 0 && transform.position.x < _maxXPos) ||
            (mouseX < 0 && transform.position.x > -_maxXPos))
        {
            transform.position += Vector3.right * mouseX * _speed * Time.deltaTime;
        }
    }

    public void DisblePaddleMovement(bool disable)
    {
        _disablePaddleMovement = disable;
    }
    public void DisblePaddleCollider(bool disable)
    {
        _polygonCollider.enabled = !disable;
    }

}
