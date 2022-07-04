using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionBoxController : MonoBehaviour, IObserver
{
    public PersonMovement playerMovement;
    public float positionOffsetX;
    public float positionOffsetY;
    
    private int _selectedPromptIndex;
    private TMP_Text _text;
    private Subject _flowSubject;
    private float _timeSinceLastPromptChange;
    private readonly List<string> _prompts = new() { "END", "GIVE" };
    private bool _active;
    
    public void Setup(Subject flowSubject)
    {
        _flowSubject = flowSubject;
        _flowSubject.AddObserver(this);
    }

    private void Update()
    {
        _timeSinceLastPromptChange += Time.deltaTime;
    }

    private void Awake()
    {
        _text = transform.Find("Text").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        Hide();
    }

    private void Hide()
    {
        _active = false;
        
        transform.localScale = Vector3.zero;
    }
    
    private void Show()
    {
        _active = true;
        _selectedPromptIndex = 0;
        
        transform.localScale = Vector3.one;
        
        Refresh();
    }
    
    private void Refresh()
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

    private void LateUpdate()
    {
        var playerPosition = playerMovement.transform.position;
        var newPosition = new Vector2(playerPosition.x + positionOffsetX, playerPosition.y + positionOffsetY);
        transform.position = newPosition;
    }
    
    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case MenuNavigation menuNavigation when _active:
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
                Hide();

                break;
            case SubjectMessage.OpenInventoryMenu when _active:
                _active = false;

                break;
            case SubjectMessage.GiveContextToInteractionMenu:
                _active = true;

                break;
            case SubjectMessage.MenuSelection when _active:
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
        void ChangePromptAnswer()
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

            _selectedPromptIndex = Mathf.Abs(_selectedPromptIndex % promptCount);

            Refresh();
        }

        Utilities.Debounce(ref _timeSinceLastPromptChange, 0.25f, ChangePromptAnswer);
    }
}
