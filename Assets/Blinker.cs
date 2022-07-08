using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinker : MonoBehaviour
{
    public Material blinkMaterial;
    private Material _defaultMaterial;
    private SpriteRenderer _sprite;
    private bool _on;
    public bool shouldBlink;
    private bool _blinking;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _defaultMaterial = _sprite.material;
    }

    private void Update()
    {
        switch (shouldBlink)
        {
            case true when !_blinking:
                _blinking = true;
                StartCoroutine(Flash(0.2f));
                break;
            case false when _blinking:
                _blinking = false;
                _sprite.material = _defaultMaterial;
                break;
        }
    }
    
    private IEnumerator Flash(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!shouldBlink) yield break;
        _sprite.material = _on ? _defaultMaterial : blinkMaterial;
        _on = !_on;

        StartCoroutine(Flash(0.2f));
    }
}