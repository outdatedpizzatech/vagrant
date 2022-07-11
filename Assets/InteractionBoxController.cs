using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionBoxController : MonoBehaviour, IObserver
{
    private int _selectedPromptIndex;
    private TMP_Text _text;
    private Subject _flowSubject;
    private readonly List<string> _prompts = new() { "END", "GIVE" };
    private Window _window;

    public void Setup(Subject flowSubject)
    {
        _flowSubject = flowSubject;
        _flowSubject.AddObserver(this);
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
        var promptIndex = 0;
        var text = "";

        foreach (var prompt in _prompts)
        {
            if (promptIndex == _selectedPromptIndex)
            {
                text += $"\n<sprite anim='0,1,4'> {prompt}";
            }
            else
            {
                text += $"\n<sprite=1> {prompt}";
            }

            promptIndex++;
        }

        _text.text = text;
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

    public void OnNotify(SubjectMessage message)
    {
        switch (message)
        {
            case SubjectMessage.OpenInteractionMenu:
                Show();

                break;
            case SubjectMessage.CloseInteractionMenu:
                _window.Hide();

                break;
            case SubjectMessage.OpenInventoryMenu when _window.IsFocused():
                _window.LoseFocus();

                break;
            case SubjectMessage.GiveContextToInteractionMenu:
                _window.Show();

                break;
            case SubjectMessage.PlayerInputConfirm when _window.IsFocused():
                switch (_selectedPromptIndex)
                {
                    case 0:
                        _flowSubject.Notify(SubjectMessage.EndInteraction);
                        break;
                    case 1:
                        _flowSubject.Notify(SubjectMessage.OpenInventoryMenu);
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