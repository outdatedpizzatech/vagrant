using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PromptController : MonoBehaviour, IObserver
{
    private Subject _flowSubject;
    private int _selectedPromptIndex;
    private float _timeSinceLastPromptChange;
    private MessageBoxController _messageBoxController;

    public void Setup(Subject flowSubject, MessageBoxController messageBoxController)
    {
        flowSubject.AddObserver(this);
        _messageBoxController = messageBoxController;
    }

    public void Reset()
    {
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
            case MenuNavigation menuNavigation when _messageBoxController.HasMessages():
                UpdatePromptSelection(menuNavigation);
                break;
        }
    }

    public string PromptContent(List<Prompt> prompts)
    {
        var promptIndex = 0;
        var text = "\n";
        
        foreach (var prompt in prompts)
        {
            if (promptIndex == _selectedPromptIndex)
            {
                text += $"\n> {prompt.Text}";
            }
            else
            {
                text += $"\n  {prompt.Text}";
            }

            promptIndex++;
        }

        return text;
    }

    private void UpdatePromptSelection(MenuNavigation menuNavigation)
    {
        void ChangePromptAnswer()
        {
            var currentMessage = _messageBoxController.CurrentMessage();
            var promptCount = currentMessage.Prompts.Count();

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

            _messageBoxController.Reload(currentMessage);
        }

        Utilities.Debounce(ref _timeSinceLastPromptChange, 0.25f, ChangePromptAnswer);
    }
}
