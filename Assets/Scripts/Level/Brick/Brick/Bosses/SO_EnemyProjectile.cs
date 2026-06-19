using UnityEngine;

[CreateAssetMenu(fileName = "SO_EnemyProjectile", menuName = "Enemy And Bosses /SO_EnemyProjectile")]
public class SO_EnemyProjectile : ScriptableObject
{
    public float _shootSpeed;
    public int _damage;

    public bool _canTakeDownShield;
    public bool _canBeDeflected;

}
