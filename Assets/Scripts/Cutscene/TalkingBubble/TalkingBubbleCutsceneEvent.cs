using FMOD.Studio;
using System.Collections;
using TMPro;
using UnityEngine;

public class TalkingBubbleCutsceneEvent : BaseCutsceneEvent
{
    [SerializeField] GameObject TalkingBubbleCutscene;

    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] float _typeSpeed;

    private EventInstance _typingLoopInstance;

    Coroutine _dialogueRoutine;
    bool _isTyping;
    bool _skipLine;
    bool _advanceLine;

    public override void ExecuteEvent()
    {
        TalkingBubbleCutscene.SetActive(true);

        if (_dialogueRoutine != null)
            StopCoroutine(_dialogueRoutine);

        _dialogueRoutine = StartCoroutine(PlayDialogue());
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

        if (_text != null)
            _text.text = string.Empty;

        TalkingBubbleCutscene.SetActive(false);

        NotifyFinished();

    }

    private void Update()
    {
        if (_dialogueRoutine == null)
            return;

        if (Input.GetMouseButtonDown(1))
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
        if (_content == null || _text == null)
            yield break;

        _text.gameObject.SetActive(true);
        _name.text = _content._speakerName;
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

        _text.text = line;
        _text.maxVisibleCharacters = 0;
        _text.ForceMeshUpdate();

        int totalVisibleCharacters = _text.textInfo.characterCount;

        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            if (_skipLine)
            {
                StopTypingSfx();
                _text.maxVisibleCharacters = totalVisibleCharacters;
                break;
            }

            _text.maxVisibleCharacters = i;
            yield return new WaitForSeconds(_typeSpeed);
        }

        _text.maxVisibleCharacters = totalVisibleCharacters;
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