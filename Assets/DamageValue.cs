using TMPro;
using UnityEngine;

public class DamageValue : MonoBehaviour
{
    private TMP_Text _text;
    private Animator _animator;
    private Subject _subject;
    
    public void Setup(Subject subject)
    {
        _subject = subject;
    }

    private void Awake()
    {
        _text = transform.Find("Text").GetComponent<TMP_Text>();
        _animator = GetComponent<Animator>();
        _text.text = "";
    }

    public void ShowDamage(int damage, Opponent opponent)
    {
        transform.position = opponent.transform.position;
        _text.text = damage.ToString();
        _animator.Play("Base Layer.TextBounce", -1, 0f);
    }

    // animation hook
    public void EndAnimation()
    {
        _subject.Notify(EncounterTopic.EndDamageAnimation);
        _text.text = "";
    }
}