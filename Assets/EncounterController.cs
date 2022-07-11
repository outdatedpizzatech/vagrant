using System.Collections.Generic;
using UnityEngine;

public class EncounterController : MonoBehaviour, IObserver
{
    private enum State
    {
        None,
        PickingAttackTarget,
        InAttackAnimation
    }

    private Subject _encounterSubject;
    private State _state;
    private State _nextState;
    private int _selectedTargetIndex;
    private readonly List<Blinker> _opponents = new();
    private AbilityAnimation _abilityAnimation;
    private Transform _opponentsTransform;
    private EventStepMarker _eventStepMarker;

    private void Update()
    {
        _state = _nextState;
    }
    
    private void Start()
    {
        foreach (Transform child in _opponentsTransform)
        {
            _opponents.Add(child.GetComponent<Blinker>());
        }
    }

    public void Setup(Subject encounterSubject, Transform opponentsTransform, AbilityAnimation abilityAnimation)
    {
        _encounterSubject = encounterSubject;
        _encounterSubject.AddObserver(this);
        _abilityAnimation = abilityAnimation;
        abilityAnimation.Setup(_encounterSubject);
        _opponentsTransform = opponentsTransform;
        _eventStepMarker = new EventStepMarker(encounterSubject);
    }

    public void OnNotify(SubjectMessage message)
    {
        switch (message)
        {
            case SubjectMessage.PickedAttack:
                SetState(State.PickingAttackTarget);
                SelectedOpponent().shouldBlink = true;
                break;
            case SubjectMessage.Cancel when _state == State.PickingAttackTarget:
                SelectedOpponent().shouldBlink = false;
                SetState(State.None);
                _encounterSubject.Notify(SubjectMessage.OpenMainMenu);
                break;
            case SubjectMessage.MenuSelection when _state == State.PickingAttackTarget && _eventStepMarker.InteractionEvent() == null:
                _encounterSubject.Notify(SubjectMessage.AttackTarget);
                break;
            case SubjectMessage.AttackTarget:
            {
                var newEvent = new InteractionEvent();
                newEvent.AddMessage("So and so attacks!");
                var response = new InteractionResponseEvent(newEvent);
                _encounterSubject.Notify(response);
                break;
            }
            case SubjectMessage.MenuSelection when _eventStepMarker.InteractionEvent() != null && _eventStepMarker.IsAtEndOfMessage():
                AdvanceEventSequence();
                break;
            case SubjectMessage.EndEventSequence when _state == State.PickingAttackTarget:
            {
                var selectedOpponent = SelectedOpponent();
                _eventStepMarker.End();
                selectedOpponent.shouldBlink = false;
                _abilityAnimation.PlaySwordAnimationOn(selectedOpponent);
                
                SetState(State.InAttackAnimation);
                break;
            }
            case SubjectMessage.EndEventSequence when _state == State.InAttackAnimation:
            {
                _eventStepMarker.End();
                SetState(State.None);
                _encounterSubject.Notify(SubjectMessage.OpenMainMenu);
                break;
            }
            case SubjectMessage.EndAttackAnimation:
            {
                var newEvent = new InteractionEvent();
                newEvent.AddMessage("A critical hit!");
                var response = new InteractionResponseEvent(newEvent);
                _encounterSubject.Notify(response);
                break;
            }
        }
    }

    private Blinker SelectedOpponent()
    {
        return _opponents[_selectedTargetIndex];
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case MenuNavigation menuNavigation when _state == State.PickingAttackTarget:
                UpdateTargetSelection(menuNavigation);
                break;
            case InteractionResponseEvent:
                ProcessEvent();
                break;
        }
    }

    private void ProcessEvent()
    {
    }

    private void UpdateTargetSelection(MenuNavigation menuNavigation)
    {
        var targetCount = _opponents.Count;

        if (targetCount == 0)
        {
            return;
        }

        SelectedOpponent().shouldBlink = false;

        switch (menuNavigation.Direction)
        {
            case Enums.Direction.Right:
                _selectedTargetIndex++;
                break;
            case Enums.Direction.Left:
                _selectedTargetIndex--;
                break;
        }

        _selectedTargetIndex = _selectedTargetIndex < 0 ? targetCount - 1 : _selectedTargetIndex % targetCount;

        SelectedOpponent().shouldBlink = true;
    }

    // Transition state on next tick to avoid message collision
    private void SetState(State newState)
    {
        _nextState = newState;
    }

    private void AdvanceEventSequence()
    {
        if (AtEndOfEvent())
        {
            _encounterSubject.Notify(SubjectMessage.EndEventSequence);
        }
        else
        {
            _eventStepMarker.StartNextEventStep();
        }
    }

    private bool AtEndOfEvent()
    {
        return _eventStepMarker.AtEndOfEvent();
    }
}