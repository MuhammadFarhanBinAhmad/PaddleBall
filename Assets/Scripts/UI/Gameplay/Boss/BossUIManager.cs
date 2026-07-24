using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUIManager : MonoBehaviour
{
    [SerializeField] GameObject _healthGameObject;
    [SerializeField] TextMeshProUGUI text_bossHealth;
    [SerializeField] Image img_bossHealth;
    [SerializeField] TextMeshProUGUI _bossName;

    int _startHealth;

    public void SetUpBossUI(string name, int value)
    {
        _bossName.text = name;
        text_bossHealth.text = value.ToString() + '/' + value.ToString();
        _startHealth = value;
    }
    public void UpdateBossHealthUI(int currHealth)
    {
        text_bossHealth.text = currHealth.ToString() + '/' + _startHealth.ToString();
        img_bossHealth.fillAmount = (float)currHealth / (float)_startHealth;
    }
    public void OpenHealthUI() => _healthGameObject.SetActive(true);
    public void CloseHealthUI() => _healthGameObject.SetActive(false);

}
