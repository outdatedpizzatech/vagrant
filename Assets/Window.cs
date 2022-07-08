using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    public PersonMovement playerMovement;
    public float positionOffsetX;
    public float positionOffsetY;

    private bool _focused;

    public void Show()
    {
        GainFocus();
        
        transform.localScale = Vector3.one;
    }
    
    private void LateUpdate()
    {
        var playerPosition = playerMovement.transform.position;
        var newPosition = new Vector2(playerPosition.x + positionOffsetX, playerPosition.y + positionOffsetY);
        transform.position = newPosition;
    }

    private void Start()
    {
        Hide();
    }

    private void BringToFront()
    {
        var currentParent = transform.parent;
        var emptyParent = GameObject.Find("Empty").transform;
        transform.SetParent(emptyParent, true);
        transform.SetParent(currentParent, true);
    }
    
    public void Hide()
    {
        LoseFocus();
        
        transform.localScale = Vector3.zero;
    }

    public bool IsFocused()
    {
        return _focused;
    }

    public void LoseFocus()
    {
        _focused = false;
    }

    public void GainFocus()
    {
        _focused = true;
        
        BringToFront();
    }
}
