using System.Collections;
using UnityEngine;

public class TestCouroutine : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(CallThis());
    }
    IEnumerator CallThis()
    {
        yield return new WaitForSeconds(3);
        print("END");
    }
}
