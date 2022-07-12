using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EncounterCommandWindowController : MonoBehaviour, IObserver
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

    public void Setup(Subject flowSubject, Subject encounterSubject, Subject interactionSubject)
    {
        _flowSubject = flowSubject;
        _encounterSubject = encounterSubject;

        _flowSubject.AddObserver(this);
        _encounterSubject.AddObserver(this);
        interactionSubject.AddObserver(this);
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case MenuNavigation menuNavigation when _window.IsFocused():
                UpdatePromptSelection(menuNavigation);
                break;
            case FlowTopic.EncounterFinishedWipeIn:
                Show();
                break;
            case FlowTopic.EncounterStartWipeOut:
                _window.Hide();
                break;
            case EncounterTopic.PickedAttack:
                _window.LoseFocus();
                RenderText();
                break;
            case EncounterTopic.OpenMainMenu:
                _window.GainFocus();
                RenderText();
                break;
            case PlayerRequestsPrimaryActionEvent when _window.IsFocused():
                switch (_selectedPromptIndex)
                {
                    case 0:
                        _encounterSubject.Notify(EncounterTopic.PickedAttack);
                        break;
                    case 1:
                        _flowSubject.Notify(FlowTopic.EncounterStartWipeOut);
                        break;
                }

                break;
        }
    }

    private void RenderText()
    {
        _text.text = Utilities.PromptOutput(_prompts, _selectedPromptIndex, _window.IsFocused());
    }

    private void UpdatePromptSelection(MenuNavigation menuNavigation)
    {
        var promptCount = _prompts.Count;
        if (promptCount < 1) return;

        _selectedPromptIndex =
            Utilities.UpdatePromptSelection(menuNavigation.Direction, _selectedPromptIndex, promptCount);

        RenderText();
    }
}