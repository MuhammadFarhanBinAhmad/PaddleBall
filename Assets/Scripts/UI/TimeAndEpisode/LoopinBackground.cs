using UnityEngine;
using UnityEngine.UI;

public class LoopinBackground : MonoBehaviour
{
    [SerializeField] float scrollSpeed = 0.1f;
    [SerializeField] float alpha = 1f;

    Material mat;
    SpriteRenderer sr;

    Vector2 offset;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // Creates a unique material instance
        mat = sr.material;
    }

    void Update()
    {
        // Scroll
        offset.x += scrollSpeed * Time.deltaTime;

        mat.SetTextureOffset(
            "_BaseMap",
            offset);

        // Alpha
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }
}
