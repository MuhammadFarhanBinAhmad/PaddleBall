using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


public class CardStoreAnim : MonoBehaviour
{
    public List<GameObject> _cardObject = new List<GameObject>();
    public List<RectTransform> _introTransforms = new List<RectTransform>();
    public List<RectTransform> _outroTransforms = new List<RectTransform>();

    Animator animator;

    [SerializeField] private float _moveDuration = 1f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void MoveCardToPosition()
    {
        StartCoroutine(DelayIntroCard());
    }
    public void MoveCardOutOfPosition()
    {
        StartCoroutine(DelayOutroCard());
    }
    IEnumerator DelayIntroCard()
    {

        int count = Mathf.Min(_cardObject.Count, _introTransforms.Count);

        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSecondsRealtime(.1f);

            RectTransform card =
                _cardObject[i].GetComponent<RectTransform>();

            StartCoroutine(
                MoveRoutine(
                    card,
                    _introTransforms[i],
                    _moveDuration));
        }
    }
    IEnumerator DelayOutroCard()
    {

        int count = Mathf.Min(_cardObject.Count, _outroTransforms.Count);

        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSecondsRealtime(.1f);

            RectTransform card =
                _cardObject[i].GetComponent<RectTransform>();

            StartCoroutine(
                MoveRoutine(
                    card,
                    _outroTransforms[i],
                    _moveDuration));
        }
    }
    IEnumerator MoveRoutine(
        RectTransform card,
        RectTransform target,
        float duration)
    {
        Vector3 startPos = card.position;
        Vector3 endPos = target.position;

        Quaternion _startRotation = card.rotation;
        Quaternion _endRotation = target.rotation;


        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(timer / duration);

            // Ease Out Cubic
            t = 1f - Mathf.Pow(1f - t, 3f);

            card.position = Vector3.Lerp(startPos, endPos, t);
            card.rotation = Quaternion.Lerp(_startRotation, _endRotation, t);
            yield return null;
        }
        card.position = endPos;
    }
}