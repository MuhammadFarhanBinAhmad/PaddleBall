using System;
using System.Collections;
using UnityEngine;

public class boss_TheApprenticeShieldManager : MonoBehaviour
{

    [SerializeField] float _timeBeforeDeactivate;

    private void OnEnable()
    {
        StartCoroutine(DeactivateShield());
    }
    IEnumerator DeactivateShield()
    {
        yield return new WaitForSeconds(_timeBeforeDeactivate);
        this.gameObject.SetActive(false);
    }

}
