using UnityEngine;
using UnityEngine.UI;

public class BrickUI : MonoBehaviour
{
    public Image [] _healthBar = new Image [5];
    int _currentLayer;

    public void PrepBrickLayerColour(int layer)
    {
        for(int i=0; i<= layer;i++)
        {
            _healthBar[i].fillAmount = 1;
        }
        for (int i = _healthBar.Length - 1; i > _currentLayer; i--)
        {
            _healthBar[i].fillAmount = 0;
        }
    }
    public void SetCurrentLayer(int layer , Color color)
    {
        _currentLayer = layer;
        _healthBar[_currentLayer].color = color;
    }
    public void SetLayerHealthFillAmount(int maxHealth, int currentHealth)
    {
        _healthBar[_currentLayer].fillAmount = (float)currentHealth / (float)maxHealth;
    }


}
