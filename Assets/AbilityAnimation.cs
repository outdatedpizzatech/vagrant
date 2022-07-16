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
        _encounterSubject.Notify(EncounterTopic.EndAttackAnimation);
    }

    public void PlayAnimation(Transform _transform, string animationName)
    {
        transform.position = _transform.position;
        _animator.Play($"Base Layer.{animationName}", -1, 0f);
    }
}