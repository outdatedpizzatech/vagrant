using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InteractionBoxController : MonoBehaviour, IObserver
{
    public PersonMovement playerMovement;
    public float positionOffset = 8;
    public PlayerController playerController;
    
    private int _selectedPromptIndex;
    private TMP_Text _text;
    private Subject _flowSubject;
    private float _timeSinceLastPromptChange;
    private List<string> _prompts = new() { "End", "Give" };
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
        
        transform.localScale = Vector3.one;
        
        Refresh();
    }
    
    private void Refresh()
    {
        var promptIndex = 0;
        var text = "\n";

        foreach (var prompt in _prompts)
        {
            if (promptIndex == _selectedPromptIndex)
            {
                text += $"\n> {prompt}";
            }
            else
            {
                text += $"\n  {prompt}";
            }

            promptIndex++;
        }
        
        _text.text = text;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var playerPosition = playerMovement.transform.position;
        var newPosition = new Vector2(playerPosition.x + positionOffset, playerPosition.y);
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
            case SubjectMessage.RequestFollowUpEvent:
                Show();

                break;
            case SubjectMessage.EndFollowUpEvent when _active:
                Hide();

                break;
            case SubjectMessage.SelectMenuItem when _active:
                if (_selectedPromptIndex == 0)
                {
                    _flowSubject.Notify(SubjectMessage.EndFollowUpEvent);
                } else if (_selectedPromptIndex == 1)
                {
                    _flowSubject.Notify(SubjectMessage.OpenMenuEvent);
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