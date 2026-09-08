using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_BrickHealthStats", menuName = "Brick/Brick Health Stats")]

[System.Serializable]
public class TierContent
{
    public int _health;
    public List<Sprite> _brickSprite = new List<Sprite>();
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
    //public List<Sprite> _brickSprite = new List<Sprite>();

}
