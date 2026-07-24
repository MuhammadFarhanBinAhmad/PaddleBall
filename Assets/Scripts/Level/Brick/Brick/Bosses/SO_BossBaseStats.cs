using UnityEngine;

[CreateAssetMenu(fileName = "SO_BossBaseStats", menuName = "Enemy And Bosses /SO_BossBaseStats")]
public class SO_BossBaseStats : ScriptableObject
{
    public string _bossName;
    public int _bossHealth;
    public float _bossSpeed;
}
