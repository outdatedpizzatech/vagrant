using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryBoxController : MonoBehaviour, IObserver
{
    public PersonMovement playerMovement;
    public float positionOffsetX;
    public float positionOffsetY;
    public PlayerController playerController;
    
    private int _selectedPromptIndex;
    private TMP_Text _text;
    private Subject _flowSubject;
    private float _timeSinceLastPromptChange;
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

    // Update is called once per frame
    void LateUpdate()
    {
        var playerPosition = playerMovement.transform.position;
        var newPosition = new Vector2(playerPosition.x + positionOffsetX, playerPosition.y + positionOffsetY);
        transform.position = newPosition;
    }
    
    public void OnNotify<T>(T parameters)
    {
        if (!_active)
        {
            return;
        }
        
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
            case SubjectMessage.OpenInventoryMenu:
                Show();

                break;
            case SubjectMessage.CloseInventoryMenu:
                Hide();

                break;
            case SubjectMessage.MenuSelection when _active:
                var selectedItem = playerController.Items()[_selectedPromptIndex];
                var selectInventoryItemEvent = new SelectInventoryItemEvent(selectedItem);
                _flowSubject.Notify(selectInventoryItemEvent);

                break;
        }
    }
    private void UpdatePromptSelection(MenuNavigation menuNavigation)
    {
        void ChangePromptAnswer()
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

            _selectedPromptIndex = Mathf.Abs(_selectedPromptIndex % promptCount);

            Refresh();
        }

        Utilities.Debounce(ref _timeSinceLastPromptChange, 0.25f, ChangePromptAnswer);
    }
}
