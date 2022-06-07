using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public class Doorway : MonoBehaviour
{
    public Doorway exit;
    public Enums.Direction exitDirection;
    public Sprite openSprite;
    
    private SpriteRenderer _spriteRenderer;
    
    // Gizmo only
    private Color? _gizmoColor = null;
    [CanBeNull] private Collider2D _gizmoCollider = null;

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
        if (exit.exitDirection == Enums.Direction.Down)
        {
            exitPosition.y -= 1;
        }
        if (exit.exitDirection == Enums.Direction.Up)
        {
            exitPosition.y += 1;
        }
        if (exit.exitDirection == Enums.Direction.Left)
        {
            exitPosition.x -= 1;
        }
        if (exit.exitDirection == Enums.Direction.Right)
        {
            exitPosition.x += 1;
        }

        return exitPosition;
    }

    void OnDrawGizmos()
    {
        if (exit != null)
        {
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

            Gizmos.color = (Color)_gizmoColor;
            
            Gizmos.DrawLine(_gizmoCollider.bounds.center, exitPosition);
            Gizmos.DrawCube(exitPosition, new Vector3(0.5f, 0.5f, 0.5f));
        }
    }
}
