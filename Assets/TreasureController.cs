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
    
    public List<MessageEnvelope> ReceiveInteraction(Enums.Direction direction)
    {
        if (_open)
        {
            return null;
        }
        
        _open = true;
        _spriteRenderer.sprite = closedSprite;

        var response1 = new MessageEnvelope
        {
            Message = "There's some old socks inside!"
        };
        
        var response2 = new MessageEnvelope
        {
            Item = "Old Socks"
        };

        return new List<MessageEnvelope>
        {
            response1,
            response2
        };
    }

    public List<MessageEnvelope> ReceiveInteraction(object promptId)
    {
        print("chest received interaction");
        return new List<MessageEnvelope>();
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
