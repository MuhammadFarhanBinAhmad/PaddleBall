using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_BrickHealthStats", menuName = "Brick/Brick Health Stats")]
public class SO_BrickHealthStats : ScriptableObject
{
    public int _parentElementID, _childElementID;
    public int _elementID;
    public int _layerNumber;
    public int _health;
    public float _dropSpeed;
    public int _APValue;
    public int _daytoUnlock;
    public Color _color;
    public List<Sprite> _brickSprite = new List<Sprite>();

}
