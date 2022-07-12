using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryWindowController : MonoBehaviour, IObserver
{
    public PlayerController playerController;

    private int _selectedPromptIndex;
    private TMP_Text _text;
    private Subject _flowSubject;
    private Window _window;
    private bool _readyForInputs;

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case MenuNavigation menuNavigation when _window.IsFocused():
                UpdatePromptSelection(menuNavigation);
                break;
            case StartEventStep when _window.IsFocused():
                _window.LoseFocus();
                break;
            case FlowTopic.OpenInventoryMenu:
                Show();

                break;
            case FlowTopic.CloseInventoryMenu:
                _window.Hide();

                break;
            case PlayerRequestsPrimaryActionEvent when _readyForInputs:
                if (playerController.Items().Any())
                {
                    var selectedItem = playerController.Items()[_selectedPromptIndex];
                    var selectInventoryItemEvent = new SelectInventoryItemEvent(selectedItem);
                    _flowSubject.Notify(selectInventoryItemEvent);
                }

                break;
            case GeneralTopic.PlayerRequestsSecondaryAction when _readyForInputs:
                _window.Hide();

                break;
        }
    }

    public bool IsVisible()
    {
        return _window.IsVisible();
    }

    public void Setup(Subject flowSubject, Subject interactionSubject, Subject contextSubject)
    {
        _flowSubject = flowSubject;
        _flowSubject.AddObserver(this);
        interactionSubject.AddObserver(this);
        _window.Setup(contextSubject);
    }

    private void Update()
    {
        _readyForInputs = _window.IsFocused();
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

    private void UpdatePromptSelection(MenuNavigation menuNavigation)
    {
        var promptCount = playerController.Items().Count;
        if (promptCount < 1) return;

        _selectedPromptIndex =
            Utilities.UpdatePromptSelection(menuNavigation.Direction, _selectedPromptIndex, promptCount);

        RenderText();
    }
}