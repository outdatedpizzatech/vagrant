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

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case DirectionalNavigation menuNavigation when _messageWindowController.IsFocused():
                UpdatePromptSelection(menuNavigation);
                break;
        }
    }

    public string PromptContent()
    {
        var text = "\n";
        var promptTexts = _prompts.Select(prompt => prompt.Text).ToList();

        text += Utilities.PromptOutput(promptTexts, _selectedPromptIndex, _messageWindowController.IsFocused());

        return text;
    }

    public Prompt SelectedPrompt()
    {
        return _prompts[_selectedPromptIndex];
    }

    private void UpdatePromptSelection(DirectionalNavigation directionalNavigation)
    {
        if (!_messageWindowController.AtEndOfCurrentMessage()) return;

        var currentEvent = _messageWindowController.InteractionEvent();
        if (currentEvent.Information is not List<Prompt> prompts || prompts.Count < 1) return;
        
        _selectedPromptIndex =
            Utilities.UpdatePromptSelection(directionalNavigation.Direction, _selectedPromptIndex, prompts.Count);
        
        _messageWindowController.RenderText();
    }
}