using UnityEngine;

[ExecuteAlways]
public class EvenSpacingGroup : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    [Header("Layout")]
    [SerializeField] private Axis axis = Axis.X;
    [SerializeField] private float spacing = 1f;
    [SerializeField] private bool centerOnParent = true;

    private void OnEnable()
    {
        ArrangeChildren();
    }

    private void OnValidate()
    {
        ArrangeChildren();
    }

    private void OnTransformChildrenChanged()
    {
        ArrangeChildren();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying)
            ArrangeChildren();
    }
#endif

    public void ArrangeChildren()
    {
        int count = transform.childCount;
        if (count == 0)
            return;

        float totalLength = (count - 1) * spacing;
        float start = centerOnParent ? -totalLength * 0.5f : 0f;

        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);

            Vector3 pos = child.localPosition;

            switch (axis)
            {
                case Axis.X:
                    pos.x = start + i * spacing;
                    break;

                case Axis.Y:
                    pos.y = start + i * spacing;
                    break;

                case Axis.Z:
                    pos.z = start + i * spacing;
                    break;
            }

            child.localPosition = pos;
        }
    }
}