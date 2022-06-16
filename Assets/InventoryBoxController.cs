using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryBoxController : MonoBehaviour, IObserver
{
    public PersonMovement playerMovement;
    public float positionOffset = 8;
    public PlayerController playerController;
    
    private TMP_Text _text;
    private Subject _flowSubject;
    
    public void Setup(Subject flowSubject)
    {
        _flowSubject = flowSubject;
        _flowSubject.AddObserver(this);
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
        transform.localScale = Vector3.zero;
    }
    
    private void Show()
    {
        transform.localScale = Vector3.one;
        
        _text.text = "";
        
        playerController.Items().ForEach((item) =>
        {
            _text.text += $"\n{item}";
        });
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
    }

    public void OnNotify(SubjectMessage message)
    {
        switch (message)
        {
            case SubjectMessage.OpenMenuEvent:
                Show();

                break;
            case SubjectMessage.CloseMenuEvent:
                Hide();

                break;
        }
    }
}
