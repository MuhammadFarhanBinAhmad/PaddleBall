using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BrickUI : MonoBehaviour
{
    public Image _healthBar;
    public List<SO_BrickHealthStats> _healthSprites = new List<SO_BrickHealthStats>();
    [SerializeField]int _currentLayer;

    public void PrepBrickLayerColour(int layer)
    {
        _currentLayer = layer;
    }
    public void UpdateHealth(int maxHealth, int currentHealth)
    {
        _healthBar.sprite = GetCurrentHealthSprite(maxHealth, currentHealth);
    }
    Sprite GetCurrentHealthSprite(int maxHealth, int currentHealth)
    {
        List<Sprite> sprites = _healthSprites[_currentLayer]._brickSprite;

        if (sprites == null || sprites.Count == 0)
            return null;

        int totalSprites = sprites.Count;
        // Clamp health to a valid range
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        float healthPercent = (float)((float)currentHealth / (float)maxHealth);
        // Convert percentage into sprite index
        int spriteIndex = Mathf.CeilToInt(healthPercent * totalSprites) - 1;

        // Clamp to valid index
        spriteIndex = Mathf.Clamp(spriteIndex, 0, totalSprites - 1);
        return sprites[spriteIndex];


    }


}
