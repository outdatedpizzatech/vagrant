using UnityEngine;

public class AbilityAnimation : MonoBehaviour
{
    private Subject _encounterSubject;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Setup(Subject encounterSubject)
    {
        _encounterSubject = encounterSubject;
    }

    public void EndAnimation()
    {
        _encounterSubject.Notify(SubjectMessage.EndAttackAnimation);
    }

    public void PlaySwordAnimationOn(Blinker opponent)
    {
        transform.position = opponent.transform.position;
        _animator.Play("Base Layer.SwordSlash", -1, 0f);
    }
}