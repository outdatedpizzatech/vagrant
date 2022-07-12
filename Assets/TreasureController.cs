using UnityEngine;

public class TreasureController : Interactable
{
    public Sprite closedSprite;
    public Item heldItem;
    
    private Subject _occupiedSpacesSubject;
    private SpriteRenderer _spriteRenderer;
    private bool _open;
    
    public override InteractionEvent ReceiveInteraction(Enums.Direction direction)
    {
        if (_open)
        {
            return null;
        }
        
        _open = true;
        _spriteRenderer.sprite = closedSprite;

        var response = new InteractionEvent();
        
        response.AddItem(heldItem);
        
        return response;
    }
    
    public void Setup(Subject occupiedSpaces)
    {
        _occupiedSpacesSubject = occupiedSpaces;
    }

    private void SetPosition(int x, int y)
    {
        _occupiedSpacesSubject.Notify(new EnteredPositionEvent(this.gameObject, x, y));
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        var currentPosition = transform.position;
        SetPosition((int)currentPosition.x, (int)currentPosition.y);
    }
}
