using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookOverlayUI : BaseOverLayInteraction
{

    [SerializeField]BookAbilityInfoUI _bookAbilityInfoUI;

    [SerializeField] Button _openBookOverlay;
    [Header("BookOverlay")]
    [SerializeField] GameObject _BookLogOverLay;
    [SerializeField] Button _spellButton, _cardButton;
    [SerializeField] Button _bookOverlyCloseButton;
    [SerializeField] Animator _bookOverlayAnimator;
    [Header("CardOverlay")]
    [SerializeField] GameObject _cardOverlay;
    [SerializeField] Button _cardOverlyCloseButton;

    [Header("SpellOverlay")]
    [SerializeField] GameObject _spellOverlay;
    [SerializeField] Button _spellOverlyCloseButton;
    [SerializeField] Button _explosionButton, _dischargeButton, _toxicButton, _critButton;
    [SerializeField] Image _abilityIcon;
    [SerializeField] TextMeshProUGUI _abilityName;
    [SerializeField] TextMeshProUGUI _abilityDescription;

    private void Start()
    {
        _openBookOverlay.onClick.AddListener(PlayOpenBookLogOverlayAnim);
        _bookOverlyCloseButton.onClick.AddListener(PlayCloseBookLogOverlayAnim);

        _spellButton.onClick.AddListener(PlayOpenSpellOverlay);
        _spellButton.onClick.AddListener(() =>
            _bookAbilityInfoUI.BuildStore(STATUSTYPE.EXPLOSION));

        _explosionButton.onClick.AddListener(() => _bookAbilityInfoUI.BuildStore(STATUSTYPE.EXPLOSION));
        _dischargeButton.onClick.AddListener(() => _bookAbilityInfoUI.BuildStore(STATUSTYPE.DISCHARGE));
        _toxicButton.onClick.AddListener(() => _bookAbilityInfoUI.BuildStore(STATUSTYPE.TOXIC));
        _critButton.onClick.AddListener(() => _bookAbilityInfoUI.BuildStore(STATUSTYPE.CRIT));


        _cardButton.onClick.AddListener(PlayOpenCardOverlayAnim);

        _cardOverlyCloseButton.onClick.AddListener(PlayCloseCardOverlayAnim);

        _spellOverlyCloseButton.onClick.AddListener(PlayCloseSpellOverlayAnim);
    }

    //BookLog
    void PlayOpenBookLogOverlayAnim()
    {
        StartCoroutine(OpenBookLogOverlay());
    }
    IEnumerator OpenBookLogOverlay()
    {
        OpenOverlay(_BookLogOverLay);
        _bookOverlayAnimator.SetTrigger("OpenBookLogOverlay");
        AnimatorStateInfo state =
        _bookOverlayAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSecondsRealtime(state.length - .1f);
    }
    void PlayCloseBookLogOverlayAnim()
    {
        StartCoroutine(CloseBookLogOverlay());
    }
    IEnumerator CloseBookLogOverlay()
    {
        _bookOverlayAnimator.SetTrigger("CloseBookLogOverlay");
        AnimatorStateInfo state =
        _bookOverlayAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSecondsRealtime(state.length - .1f);
        CloseOverlay(_BookLogOverLay);
    }

    //CardOverlay
    void PlayOpenCardOverlayAnim()
    {
        StartCoroutine(OpenCardOverlay());
    }
    IEnumerator OpenCardOverlay()
    {
        _bookOverlayAnimator.SetTrigger("OpenCardOverlay");
        AnimatorStateInfo state =
        _bookOverlayAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSecondsRealtime(.35f);
        OpenOverlay(_cardOverlay);
    }
    void PlayCloseCardOverlayAnim()
    {
        StartCoroutine(CloseCardOverlay());
    }
    IEnumerator CloseCardOverlay()
    {
        _bookOverlayAnimator.SetTrigger("CloseCardOverlay");
        AnimatorStateInfo state =
        _bookOverlayAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSecondsRealtime(.35f);
        CloseOverlay(_cardOverlay);
        TimeManager.StopTime();
    }    
    //SpellOverlay
    void PlayOpenSpellOverlay()
    {
        StartCoroutine(OpenSpellOverlay());
    }
    IEnumerator OpenSpellOverlay()
    {
        _bookOverlayAnimator.SetTrigger("OpenSpellOverlay");
        AnimatorStateInfo state =
        _bookOverlayAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSecondsRealtime(.95f);
        OpenOverlay(_spellOverlay);
    }
    void PlayCloseSpellOverlayAnim()
    {
        StartCoroutine(CloseSpellOverlay());

    }
    IEnumerator CloseSpellOverlay()
    {
        _bookOverlayAnimator.SetTrigger("CloseSpellOverlay");
        AnimatorStateInfo state =
        _bookOverlayAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSecondsRealtime(.95f);
        CloseOverlay(_spellOverlay);
        TimeManager.StopTime();
    }
    public void SetAbilityDetail(SOStoreAbilityContent content)
    {
        print("set");
        _abilityIcon.sprite = content.icon;
        _abilityName.text = content.ability_Name;
        _abilityDescription.text = content.ability_Description;
    }
    public void ClearAbilityDetail()
    {
        _abilityIcon.sprite = null;
        _abilityName.text = "";
        _abilityDescription.text = "";
    }
}
