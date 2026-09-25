using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_BrickHealthStats", menuName = "Brick/Brick Health Stats")]

[System.Serializable]
public class TierContent
{
    public int _health;
    public List<Sprite> _brickSprite = new List<Sprite>();
}
[System.Flags]
public enum ABILITY
{
    NONE = 0,
    SHOOT_PROJECTILE_01 = 1 << 1,
    DEFENSE_01 = 1 << 2,
    SUPPORT_01 = 1 << 3,
}
public class SO_BrickHealthStats : ScriptableObject
{

    public int _parentElementID, _childElementID;
    public int _elementID;
    public int _layerNumber;
    //public int _health;
    public float _dropSpeed;
    public int _APValue;
    public int _daytoUnlock;
    public List<TierContent> _tiers = new List<TierContent>();
    public ABILITY _ability;
    //public List<Sprite> _brickSprite = new List<Sprite>();

}
