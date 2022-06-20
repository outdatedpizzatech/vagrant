using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class TreasureController : MonoBehaviour, IInteractable
{
    private Subject _occupiedSpacesSubject;
    public Sprite closedSprite;
    private SpriteRenderer _spriteRenderer;
    private bool _open;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        var currentPosition = transform.position;
        SetPosition((int)currentPosition.x, (int)currentPosition.y);
    }
    
    public InteractionEvent ReceiveInteraction(Enums.Direction direction)
    {
        if (_open)
        {
            return null;
        }
        
        _open = true;
        _spriteRenderer.sprite = closedSprite;

        var response = new InteractionEvent();
        
        response.AddMessage("There's some old socks inside!");
        response.AddItem("Old Socks");
        
        return response;
    }

    public InteractionEvent ReceiveInteraction(object promptId)
    {
        return null;
    }

    public void SetPosition(int x, int y)
    {
        _occupiedSpacesSubject.Notify(new EnteredPositionEvent(this.gameObject, x, y));
    }
    
    public void Setup(Subject occupiedSpaces)
    {
        _occupiedSpacesSubject = occupiedSpaces;
    }
}
