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

    public MessageWindowController messageWindowController;
    private Subject _encounterSubject;
    private State _state;
    private State _nextState;
    private int _selectedTargetIndex;
    private readonly List<Blinker> _opponents = new();
    private AbilityAnimation _abilityAnimation;
    private Transform _opponentsTransform;
    private EventStepMarker _eventStepMarker;

    public void Setup(Subject encounterSubject, Transform opponentsTransform, AbilityAnimation abilityAnimation,
        Subject interactionSubject)
    {
        _encounterSubject = encounterSubject;
        _encounterSubject.AddObserver(this);
        _abilityAnimation = abilityAnimation;
        abilityAnimation.Setup(_encounterSubject);
        _opponentsTransform = opponentsTransform;
        _eventStepMarker = new EventStepMarker(encounterSubject, messageWindowController, interactionSubject);
        interactionSubject.AddObserver(this);
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case DirectionalNavigation menuNavigation when _state == State.PickingAttackTarget:
                UpdateTargetSelection(menuNavigation);
                break;
            case EncounterTopic.PickedAttack:
                SetState(State.PickingAttackTarget);
                SelectedOpponent().shouldBlink = true;
                break;
            case EncounterTopic.Cancel when _state == State.PickingAttackTarget:
                SelectedOpponent().shouldBlink = false;
                SetState(State.None);
                _encounterSubject.Notify(EncounterTopic.OpenMainMenu);
                break;
            case PlayerRequestsPrimaryActionEvent
                when _state == State.PickingAttackTarget && !_eventStepMarker.Active():
                _encounterSubject.Notify(EncounterTopic.AttackTarget);
                break;
            case GeneralTopic.PlayerRequestsSecondaryAction:
                _encounterSubject.Notify(EncounterTopic.Cancel);
                break;
            case EncounterTopic.AttackTarget:
            {
                var newEvent = new InteractionEvent();
                newEvent.AddMessage("So and so attacks!");
                var response = new InteractionResponseEvent(newEvent);
                _encounterSubject.Notify(response);
                break;
            }
            case EventTopic.EndEventSequence when _state == State.PickingAttackTarget:
            {
                var selectedOpponent = SelectedOpponent();
                selectedOpponent.shouldBlink = false;
                _abilityAnimation.PlaySwordAnimationOn(selectedOpponent);

                SetState(State.InAttackAnimation);
                break;
            }
            case EventTopic.EndEventSequence when _state == State.InAttackAnimation:
            {
                SetState(State.None);
                _encounterSubject.Notify(EncounterTopic.OpenMainMenu);
                break;
            }
            case EncounterTopic.EndAttackAnimation:
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

    private void UpdateTargetSelection(DirectionalNavigation directionalNavigation)
    {
        var targetCount = _opponents.Count;

        if (targetCount == 0)
        {
            return;
        }

        SelectedOpponent().shouldBlink = false;

        switch (directionalNavigation.Direction)
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
}