using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TEXTBOX_TYPE
{
    TUTORIAL,
    BOSS,
    PLAYER
}
[System.Serializable]
public class TextBoxContent
{
    public GameObject TalkingBubbleCutscene;
    public TextMeshProUGUI _name;
    public TextMeshProUGUI _text;
    public float _typeSpeed;
}

[System.Serializable]
public class TextBoxType
{
    public TEXTBOX_TYPE textbox_type;
    public TextBoxContent content;
}

public class TalkingBubbleCutsceneEvent : BaseCutsceneEvent
{
    [SerializeField] WorldToCanvasPosition _worldToCanvasPosition;

    [SerializeField] public List<TextBoxType> _TextBoxType;
    TextBoxContent _currTextBoxContent;


    private EventInstance _typingLoopInstance;

    Coroutine _dialogueRoutine;
    bool _isTyping;
    bool _skipLine;
    bool _advanceLine;

    public override void ExecuteEvent()
    {

        foreach (var t in _TextBoxType)
        {
            t.content.TalkingBubbleCutscene.gameObject.SetActive(false);
        }

        switch (_content.TEXTBOX_TYPE)
        {
            case TEXTBOX_TYPE.TUTORIAL:
                {
                    _currTextBoxContent = _TextBoxType[0].content;
                    break;
                }
            case TEXTBOX_TYPE.BOSS:
                {
                    _currTextBoxContent = _TextBoxType[1].content;
                    break;
                }
            case TEXTBOX_TYPE.PLAYER:
                {
                    _currTextBoxContent = _TextBoxType[2].content;
                    break;
                }
        }
        _worldToCanvasPosition._textBox = _currTextBoxContent.TalkingBubbleCutscene.GetComponent<RectTransform>();
        _currTextBoxContent.TalkingBubbleCutscene.gameObject.SetActive(true);

        if (_dialogueRoutine != null)
            StopCoroutine(_dialogueRoutine);

        _dialogueRoutine = StartCoroutine(PlayDialogue());
        if (_content._freezeGame)
            TimeManager.StopTime();
    }

    public override void EndEvent()
    {
        StopTypingSfx();

        if (_dialogueRoutine != null)
        {
            StopCoroutine(_dialogueRoutine);
            _dialogueRoutine = null;
        }

        _isTyping = false;
        _skipLine = false;
        _advanceLine = false;

        if (_currTextBoxContent._text != null)
            _currTextBoxContent._text.text = string.Empty;

        _currTextBoxContent.TalkingBubbleCutscene.gameObject.SetActive(false);
        if (_content._freezeGame)
            TimeManager.ResetTimeScale();
        NotifyFinished();

    }

    private void Update()
    {
        if (_dialogueRoutine == null)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isTyping)
            {
                _skipLine = true;
            }
            else
            {
                _advanceLine = true;
            }
        }
    }

    private IEnumerator PlayDialogue()
    {
        if (_content == null || _currTextBoxContent._text == null)
            yield break;

        _currTextBoxContent._text.gameObject.SetActive(true);
        _currTextBoxContent._name.text = _content._speakerName;
        for (int i = 0; i < _content._dialougeTexts.Count; i++)
        {
            yield return StartCoroutine(TypeLine(_content._dialougeTexts[i]));

            _advanceLine = false;
            while (!_advanceLine)
                yield return null;

            _advanceLine = false;
        }

        EndEvent();
    }

    private IEnumerator TypeLine(string line)
    {
        _isTyping = true;
        _skipLine = false;
        StartTypingSfx();

        _currTextBoxContent._text.text = line;
        _currTextBoxContent._text.maxVisibleCharacters = 0;
        _currTextBoxContent._text.ForceMeshUpdate();

        int totalVisibleCharacters = _currTextBoxContent._text.textInfo.characterCount;

        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            if (_skipLine)
            {
                StopTypingSfx();
                _currTextBoxContent._text.maxVisibleCharacters = totalVisibleCharacters;
                break;
            }

            _currTextBoxContent._text.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(_currTextBoxContent._typeSpeed);
        }

        _currTextBoxContent._text.maxVisibleCharacters = totalVisibleCharacters;
        _isTyping = false;
        StopTypingSfx();
    }

    private void StartTypingSfx()
    {
        if (FmodEvent.Instance.sfx_paddleText.IsNull)
            return;

        StopTypingSfx();

        _typingLoopInstance = AudioManager.Instance.CreateEventInstance(FmodEvent.Instance.sfx_paddleText);
        _typingLoopInstance.start();
    }

    private void StopTypingSfx()
    {
        if (!_typingLoopInstance.isValid())
            return;

        _typingLoopInstance.stop(STOP_MODE.ALLOWFADEOUT);
        _typingLoopInstance.release();
        _typingLoopInstance.clearHandle();
    }
}