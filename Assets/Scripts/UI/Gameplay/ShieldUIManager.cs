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
        _deadZone.OnShieldRecharging += UpdateShieldUI;
    }
    private void OnDestroy()
    {
        _deadZone.OnShieldDamage -= UpdateShieldUI;
        _deadZone.OnShieldRecharging -= UpdateShieldUI;

    }
    void UpdateShieldUI()
    {
        _manaImage.fillAmount = _deadZone.GetShieldPercentage();
    }
}
