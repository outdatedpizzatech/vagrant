using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PromptController : MonoBehaviour, IObserver
{
    private int _selectedPromptIndex;
    private List<Prompt> _prompts;
    private MessageWindowController _messageWindowController;

    public void Setup(Subject subject, MessageWindowController messageWindowController)
    {
        subject.AddObserver(this);
        _messageWindowController = messageWindowController;
    }

    public void ResetPrompts(List<Prompt> prompts)
    {
        _prompts = prompts;
        _selectedPromptIndex = 0;
    }

    public void OnNotify(SubjectMessage message)
    {
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case MenuNavigation menuNavigation when _messageWindowController.IsFocused():
                UpdatePromptSelection(menuNavigation);
                break;
        }
    }

    public string PromptContent()
    {
        var text = "\n";
        var promptTexts = _prompts.Select(prompt => prompt.Text).ToList();

        text += Utilities.PromptOutput(promptTexts, _selectedPromptIndex);

        return text;
    }

    public Prompt SelectedPrompt()
    {
        return _prompts[_selectedPromptIndex];
    }

    private void UpdatePromptSelection(MenuNavigation menuNavigation)
    {
        var currentEvent = _messageWindowController.InteractionEvent();

        var promptCount = 0;

        if (currentEvent.Information is List<Prompt> prompts)
        {
            promptCount = prompts.Count();
        }

        if (promptCount <= 0 || !_messageWindowController.AtEndOfCurrentMessage()) return;

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

        _messageWindowController.RenderText();
    }
}