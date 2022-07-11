using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryBoxController : MonoBehaviour, IObserver
{
    public PlayerController playerController;

    private int _selectedPromptIndex;
    private TMP_Text _text;
    private Subject _flowSubject;
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

        RenderText();
    }

    private void RenderText()
    {
        var promptIndex = 0;
        var text = "";

        foreach (var prompt in playerController.Items().Select((i) => i.itemName))
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
        if (!_window.IsFocused())
        {
            return;
        }

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
            case SubjectMessage.OpenInventoryMenu:
                Show();

                break;
            case SubjectMessage.CloseInventoryMenu:
                _window.Hide();

                break;
            case SubjectMessage.PlayerInputConfirm when _window.IsFocused():
                var selectedItem = playerController.Items()[_selectedPromptIndex];
                var selectInventoryItemEvent = new SelectInventoryItemEvent(selectedItem);
                _flowSubject.Notify(selectInventoryItemEvent);

                break;
        }
    }

    private void UpdatePromptSelection(MenuNavigation menuNavigation)
    {
        var promptCount = playerController.Items().Count;

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