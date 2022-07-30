using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HpBox : MonoBehaviour
{
    public List<Damageable> _damageables = new();

    private TMP_Text _text;
    private RectTransform _rectTransform;

    public void AddDamageable(Damageable damageable)
    {
        _damageables.Add(damageable);
        _rectTransform.sizeDelta = new Vector2(_damageables.Count * 2 + 1, 1.5f);
    }

    // TODO: use observables
    public void Clear()
    {
        _damageables.Clear();
    }

    private void Awake()
    {
        _text = transform.Find("Text").GetComponent<TMP_Text>();
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        var text = "";

        int currentIndex = 0;

        foreach (Damageable damageable in _damageables)
        {
            if (currentIndex > 0)
            {
                text += " ";
            }

            text += $"{damageable.hitPoints:000}";

            currentIndex++;
        }

        _text.text = text;
    }
}