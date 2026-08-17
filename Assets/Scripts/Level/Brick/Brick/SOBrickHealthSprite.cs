using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOBrickHealthSprite", menuName = "Brick/SOBrickHealthSprite")]
public class SOBrickHealthSprite : ScriptableObject
{
    public List<Sprite> _brickSprite = new List<Sprite>();
}
