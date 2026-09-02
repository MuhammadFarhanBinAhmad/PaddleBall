using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchasedCardContent : MonoBehaviour
{
    [Header("Content")]
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] TextMeshProUGUI _description;
    [SerializeField] Image _logo;

    public void SetCard(SOItemAbilityContentUI content)
    {
        if (content == null)
            return;

        _name.text = content.ability_Name;
        _description.text = content.ability_Description;
        _logo.sprite = content.icon;
    }
}
