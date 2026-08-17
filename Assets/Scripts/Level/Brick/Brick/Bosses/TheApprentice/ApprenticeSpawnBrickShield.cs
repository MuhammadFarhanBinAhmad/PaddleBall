using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApprenticeSpawnBrickShield : MonoBehaviour
{

    [SerializeField] SO_BrickHealthStats _stats;

    BrickPool _brickPool;
    public List<Transform> _pos = new List<Transform>();

    private void Awake()
    {
        _brickPool = FindAnyObjectByType<BrickPool>();
    }
    public void StartSpawningShield()
    {
        StartCoroutine(SpawnBrick());
    }
    IEnumerator SpawnBrick()
    {
        foreach (Transform t in _pos)
        {
            yield return new WaitForSeconds(.25f);
            GameObject _bb = _brickPool.GetBrick();
            _bb.GetComponent<BrickBar>().SetBrick(_stats);
            _bb.transform.position = t.position;
        }
    }
}
