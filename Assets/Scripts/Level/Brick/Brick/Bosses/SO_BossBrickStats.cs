using UnityEngine;
[CreateAssetMenu(fileName = "SO_BossBrickStats", menuName = "Brick/Boss Brick Stats")]

public class SO_BossBrickStats : ScriptableObject
{
    public GameObject _bossPrefab;
    public string _bossName;
    public int _health;
    public float _moveSpeed;

}
