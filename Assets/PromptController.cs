using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PromptController : MonoBehaviour, IObserver
{
    private Subject _flowSubject;
    private int _selectedPromptIndex;
    private List<Prompt> _prompts;
    private float _timeSinceLastPromptChange;
    private MessageBoxController _messageBoxController;

    public void Setup(Subject flowSubject, MessageBoxController messageBoxController)
    {
        flowSubject.AddObserver(this);
        _messageBoxController = messageBoxController;
    }

    public void ResetPrompts(List<Prompt> prompts)
    {
        _prompts = prompts;
        _selectedPromptIndex = 0;
    }

    public void OnNotify(SubjectMessage message)
    {
    }

    private void Update()
    {
        _timeSinceLastPromptChange += Time.deltaTime;
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case MenuNavigation menuNavigation when _messageBoxController.IsFocused():
                UpdatePromptSelection(menuNavigation);
                break;
        }
    }

    public string PromptContent()
    {
        var promptIndex = 0;
        var text = "\n";
        
        foreach (var prompt in _prompts)
        {
            if (promptIndex == _selectedPromptIndex)
            {
                text += $"\n<sprite anim='0,1,4'> {prompt.Text}";
            }
            else
            {
                text += $"\n<sprite=1> {prompt.Text}";
            }

            promptIndex++;
        }

        return text;
    }

    public Prompt SelectedPrompt()
    {
        return _prompts[_selectedPromptIndex];
    }

    private void UpdatePromptSelection(MenuNavigation menuNavigation)
    {
        void ChangePromptAnswer()
        {
            var currentEvent = _messageBoxController.InteractionEvent();
            var promptCount = currentEvent.Prompts.Count();

            if (promptCount <= 0 || !_messageBoxController.AtEndOfCurrentMessage()) return;

            switch (menuNavigation.Direction)
            {
                case Enums.Direction.Down:
                    _selectedPromptIndex++;
                    break;
                case Enums.Direction.Up:
                    _selectedPromptIndex--;
                    break;
            }

            _selectedPromptIndex = Mathf.Abs(_selectedPromptIndex % promptCount);

            _messageBoxController.RenderText();
        }

        Utilities.Debounce(ref _timeSinceLastPromptChange, 0.25f, ChangePromptAnswer);
    }
}
