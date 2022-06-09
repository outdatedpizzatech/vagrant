using UnityEngine;
using System.Collections;

public class Doorway : MonoBehaviour
{
    public Doorway exit;
    public Enums.Direction exitDirection;
    public Sprite openSprite;
    
    private SpriteRenderer _spriteRenderer;
    
    // Gizmo only
    private Color? _gizmoColor;
    private Collider2D _gizmoCollider;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collider2d)
    {
        var player = collider2d.GetComponent<PlayerWarp>();
        var exitPosition = ExitPosition();
        
        // TODO: * Consider making this event-driven
        player.WarpTo(Mathf.FloorToInt(exitPosition.x), Mathf.FloorToInt(exitPosition.y), exitDirection);

        if (!openSprite)
        {
            return;
        }
        
        StartCoroutine(CloseDoor(0.5f, _spriteRenderer.sprite));
        _spriteRenderer.sprite = openSprite;
    }

    private IEnumerator CloseDoor(float waitTime, Sprite sprite)
    {
        yield return new WaitForSeconds(waitTime);
        
        _spriteRenderer.sprite = sprite;
    }

    private Vector2 ExitPosition()
    {
        Vector2 exitPosition = exit.transform.position;
        switch (exit.exitDirection)
        {
            case Enums.Direction.Down:
                exitPosition.y -= 1;
                break;
            case Enums.Direction.Up:
                exitPosition.y += 1;
                break;
            case Enums.Direction.Left:
                exitPosition.x -= 1;
                break;
            case Enums.Direction.Right:
                exitPosition.x += 1;
                break;
        }

        return exitPosition;
    }

    private void OnDrawGizmos()
    {
        if (exit == null) return;
        if (_gizmoCollider == null)
        {
            _gizmoCollider = GetComponent<Collider2D>();   
        }
            
        if (_gizmoColor == null)
        {
            var h = Random.Range(0, 1f);
                
            _gizmoColor = Color.HSVToRGB(h, 1, 1);
        }

        var exitPosition = ExitPosition() + new Vector2(0.5f, 0.5f);

        if (_gizmoColor != null) Gizmos.color = (Color)_gizmoColor;

        if (_gizmoCollider != null) Gizmos.DrawLine(_gizmoCollider.bounds.center, exitPosition);
        Gizmos.DrawCube(exitPosition, new Vector3(0.5f, 0.5f, 0.5f));
    }
}
