using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShieldUIManager : MonoBehaviour
{
    DeadZone _deadZone;

    [SerializeField] Image _manaImage;
    [SerializeField] TextMeshProUGUI _health;

    private void Awake()
    {
        _deadZone = FindAnyObjectByType<DeadZone>();
    }
    private void Start()
    {
        _deadZone.OnShieldDamage += UpdateShieldUI;

        UpdateShieldUI();
    }
    private void OnDestroy()
    {
        _deadZone.OnShieldDamage -= UpdateShieldUI;

    }
    public void UpdateShieldUI()
    {
        _health.text =
            Mathf.RoundToInt(_deadZone.GetCurrentShield()).ToString()
            + "/"
            + Mathf.RoundToInt(_deadZone.GetMaxShield()).ToString();
        _manaImage.fillAmount = _deadZone.GetShieldPercentage();
    }
}
