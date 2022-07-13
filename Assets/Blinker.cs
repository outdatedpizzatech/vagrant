using System.Collections;
using UnityEngine;

public class Blinker : MonoBehaviour
{
    public Material blinkMaterial;
    private Material _defaultMaterial;
    private SpriteRenderer _sprite;
    private bool _on;
    public bool shouldBlink;
    private bool _blinking;
    private float _flashFor;
    private float _flashTimer;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _defaultMaterial = _sprite.material;
    }

    private void Update()
    {
        if (_flashFor > 0)
        {
            _flashTimer += Time.deltaTime;
            if (_flashTimer < _flashFor)
            {
                shouldBlink = true;
            }
            else
            {
                _flashFor = 0;
                shouldBlink = false;
            }
        }

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

    public void FlashFor(float duration)
    {
        shouldBlink = true;
        _flashFor = duration;
        _flashTimer = 0;
    }
}