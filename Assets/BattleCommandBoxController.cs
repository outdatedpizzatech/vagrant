using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleCommandBoxController : MonoBehaviour, IObserver
{
    private int _selectedPromptIndex;
    private Window _window;
    private Subject _flowSubject;
    private Subject _encounterSubject;
    private readonly List<string> _prompts = new() { "ATTACK", "FLEE" };
    private TMP_Text _text;

    private void Show()
    {
        _window.Show();
        _selectedPromptIndex = 0;
        RenderText();
    }

    private void Awake()
    {
        _window = GetComponent<Window>();
        _text = transform.Find("Text").GetComponent<TMP_Text>();
    }

    public void Setup(Subject flowSubject, Subject encounterSubject)
    {
        _flowSubject = flowSubject;
        _encounterSubject = encounterSubject;

        _flowSubject.AddObserver(this);
        _encounterSubject.AddObserver(this);
    }

    public void OnNotify(SubjectMessage message)
    {
        switch (message)
        {
            case SubjectMessage.EncounterFinishedWipeIn:
                Show();
                break;
            case SubjectMessage.EncounterStartWipeOut:
                _window.Hide();
                break;
            case SubjectMessage.PickedAttack:
                _window.LoseFocus();
                RenderText();
                break;
            case SubjectMessage.OpenMainMenu:
                _window.GainFocus();
                RenderText();
                break;
            case SubjectMessage.MenuSelection when _window.IsFocused():
                switch (_selectedPromptIndex)
                {
                    case 0:
                        _encounterSubject.Notify(SubjectMessage.PickedAttack);
                        break;
                    case 1:
                        _flowSubject.Notify(SubjectMessage.EncounterStartWipeOut);
                        break;
                }
                break;
        }
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case MenuNavigation menuNavigation when _window.IsFocused():
                UpdatePromptSelection(menuNavigation);
                break;
        }
    }

    private void RenderText()
    {
        var promptIndex = 0;
        var text = "";

        foreach (var prompt in _prompts)
        {
            if (promptIndex == _selectedPromptIndex)
            {
                if (_window.IsFocused())
                {
                    text += $"\n<sprite anim='0,1,4'> {prompt}";
                }else{
                    text += $"\n<sprite=0> {prompt}";
                }
            }
            else
            {
                text += $"\n<sprite=1> {prompt}";
            }

            promptIndex++;
        }

        _text.text = text;
    }
    
    private void UpdatePromptSelection(MenuNavigation menuNavigation)
    {
        var promptCount = _prompts.Count;

        if (promptCount == 0)
        {
            return;
        }

        switch (menuNavigation.Direction)
        {
            case Enums.Direction.Down:
                _selectedPromptIndex++;
                break;
            case Enums.Direction.Up:
                _selectedPromptIndex--;
                break;
        }

        _selectedPromptIndex = _selectedPromptIndex < 0 ? promptCount - 1 : _selectedPromptIndex % promptCount;

        RenderText();
    }
}