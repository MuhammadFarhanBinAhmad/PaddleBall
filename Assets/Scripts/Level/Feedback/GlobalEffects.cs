using System.Collections;
using UnityEngine;

public class GlobalEffects : MonoBehaviour
{
    public static GlobalEffects Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            print("more than one instance in the scene");

        Instance = this;
    }
    public void PlayLerpObject(GameObject obj, SO_BrickSpecialEffect _bse)
    {
        StartCoroutine(LerpObject(obj, _bse));
    }

    IEnumerator LerpObject(GameObject obj, SO_BrickSpecialEffect _bse)
    {
        Vector3 startScale;
        Vector3 endScale ;
        Vector3 _origanalSize = obj.transform.localScale;

        if (_bse.ReferenceObjectScale)
        {

            startScale = _origanalSize * _bse._startScaleMultiplier;
            endScale = _origanalSize * _bse._endScaleMultiplier;
        }
        else
        {
            startScale = new Vector3(_bse._startScaleMultiplier, _bse._startScaleMultiplier, _bse._startScaleMultiplier);
            endScale = new Vector3(_bse._endScaleMultiplier, _bse._endScaleMultiplier, _bse._endScaleMultiplier);
        }

        obj.transform.localScale = startScale;


        float time = 0f;
        while (time < _bse._lerpDuration)
        {
            float t = time / _bse._lerpDuration;
            float curveT = _bse._effectLerp != null
                ? _bse._effectLerp.Evaluate(t)
                : t;

            obj.transform.localScale =
                Vector3.LerpUnclamped(startScale, endScale, curveT);

            time += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = endScale;
    }
}
