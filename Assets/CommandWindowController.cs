using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommandWindowController : MonoBehaviour, IObserver
{
    private int _selectedPromptIndex;
    private TMP_Text _text;
    private Subject _flowSubject;
    private readonly List<string> _prompts = new() { "END", "GIVE" };
    private Window _window;

    public void Setup(Subject flowSubject, Subject interactionSubject, Subject windowSubject)
    {
        _flowSubject = flowSubject;
        _flowSubject.AddObserver(this);
        interactionSubject.AddObserver(this);
        _window.Setup(windowSubject);
    }

    private void Awake()
    {
        _window = GetComponent<Window>();
        _text = transform.Find("Text").GetComponent<TMP_Text>();
    }

    private void Show()
    {
        _window.Show();
        _selectedPromptIndex = 0;
        RenderText();
    }

    private void RenderText()
    {
        _text.text = Utilities.PromptOutput(_prompts, _selectedPromptIndex);
    }

    public bool IsVisible()
    {
        return _window.IsVisible();
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case MenuNavigation menuNavigation when _window.IsFocused():
                UpdatePromptSelection(menuNavigation);
                break; 
            case FlowTopic.OpenCommandWindow:
                Show();

                break;
            case FlowTopic.CloseCommandWindow:
                _window.Hide();

                break;
            case FlowTopic.OpenInventoryMenu when _window.IsFocused():
                _window.LoseFocus();

                break;
            case PlayerRequestsPrimaryActionEvent when _window.IsFocused():
                switch (_selectedPromptIndex)
                {
                    case 0:
                        _flowSubject.Notify(FlowTopic.EndInteraction);
                        break;
                    case 1:
                        _flowSubject.Notify(FlowTopic.OpenInventoryMenu);
                        break;
                }

                break;
        }
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