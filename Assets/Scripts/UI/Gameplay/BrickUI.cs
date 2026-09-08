using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BrickUI : MonoBehaviour
{
    public Image _healthBar;
    public List<SO_BrickHealthStats> _healthSprites = new List<SO_BrickHealthStats>();

    [SerializeField] int _currentLayer;

    [SerializeField] List<Sprite> _currentSprites = new List<Sprite>();

    public void PrepBrickLayerColour(int layer)
    {
        _currentLayer = layer;

        _currentSprites.Clear();

        foreach (SO_BrickHealthStats stats in _healthSprites)
        {
            if (stats != null && stats._layerNumber == _currentLayer)
            {
                // If you only want one tier, handle that here.
                // Otherwise add sprites from all tiers.
                foreach (Sprite sprite in stats._tiers[layer]._brickSprite)
                {
                    _currentSprites.Add(sprite);
                }

                break;
            }
        }
    }

    public void UpdateHealth(int maxHealth, int currentHealth)
    {
        _healthBar.sprite = GetCurrentHealthSprite(maxHealth, currentHealth);
    }

    Sprite GetCurrentHealthSprite(int maxHealth, int currentHealth)
    {
        if (_currentSprites == null || _currentSprites.Count == 0)
            return null;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        float healthPercent = (float)currentHealth / maxHealth;

        int spriteIndex =
            Mathf.CeilToInt(
                healthPercent * _currentSprites.Count
            ) - 1;

        spriteIndex = Mathf.Clamp(
            spriteIndex,
            0,
            _currentSprites.Count - 1
        );

        return _currentSprites[spriteIndex];
    }
}