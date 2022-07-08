using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAnimation : MonoBehaviour
{
    private Subject _encounterSubject;

    public void Setup(Subject encounterSubject)
    {
        _encounterSubject = encounterSubject;
    }

    public void EndAnimation()
    {
        _encounterSubject.Notify(SubjectMessage.EndAttackAnimation);
    }
}