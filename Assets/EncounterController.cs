using System.Collections.Generic;
using UnityEngine;

class Attack
{
    public int Damage;
    public Opponent Opponent;
    public bool IsCritical;

    public Attack(Opponent opponent)
    {
        Opponent = opponent;
        Damage = Random.Range(4, 9);

        if (Random.Range(0f, 1f) > 0.8f)
        {
            IsCritical = true;
            Damage *= 3;
        }
    }
}

public class EncounterController : MonoBehaviour, IObserver
{
    private enum State
    {
        None,
        PickingAttackTarget,
        StartingAttack,
        InAttackAnimation,
        PostAnimationMessage,
        ShowDamageValues,
        NegotiatingDamage,
        ResolveTurn,
        EndingEncounter,
    }

    public MessageWindowController messageWindowController;
    private Subject _encounterSubject;
    private Subject _flowSubject;
    private State _state;
    private State _nextState;
    private int _selectedTargetIndex;
    private readonly List<Opponent> _opponents = new();
    private AbilityAnimation _abilityAnimation;
    private DamageValue _damageValue;
    private Transform _opponentsTransform;
    private EventStepMarker _eventStepMarker;
    private Attack _attack;

    public void Setup(Subject encounterSubject, Transform opponentsTransform, AbilityAnimation abilityAnimation,
        Subject interactionSubject, DamageValue damageValue, Subject flowSubject)
    {
        _encounterSubject = encounterSubject;
        _encounterSubject.AddObserver(this);
        _abilityAnimation = abilityAnimation;
        abilityAnimation.Setup(_encounterSubject);
        _opponentsTransform = opponentsTransform;
        _eventStepMarker = new EventStepMarker(encounterSubject, messageWindowController, interactionSubject);
        interactionSubject.AddObserver(this);
        _damageValue = damageValue;
        _damageValue.Setup(_encounterSubject);
        _flowSubject = flowSubject;
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
                SelectedOpponent().GetComponent<Blinker>().shouldBlink = true;
                break;
            case EncounterTopic.Cancel when _state == State.PickingAttackTarget:
                SelectedOpponent().GetComponent<Blinker>().shouldBlink = false;
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
                _attack = new Attack(SelectedOpponent());
                SetState(State.StartingAttack);
                var newEvent = new InteractionEvent();
                newEvent.AddMessage("So and so attacks!");
                var response = new InteractionResponseEvent(newEvent);
                _encounterSubject.Notify(response);
                break;
            }
            case EventTopic.EndEventSequence when _state == State.StartingAttack:
            {
                var selectedOpponent = SelectedOpponent();
                selectedOpponent.GetComponent<Blinker>().shouldBlink = false;
                _abilityAnimation.PlaySwordAnimationOn(selectedOpponent);

                SetState(State.InAttackAnimation);
                break;
            }
            case EncounterTopic.EndAttackAnimation:
            {
                SetState(State.PostAnimationMessage);
                if (_attack.IsCritical)
                {
                    var newEvent = new InteractionEvent();
                    newEvent.AddMessage("A critical hit!");
                    var response = new InteractionResponseEvent(newEvent);
                    _encounterSubject.Notify(response);
                }

                break;
            }
            case EventTopic.EndEventSequence when _state == State.InAttackAnimation:
            {
                SetState(State.None);
                _encounterSubject.Notify(EncounterTopic.OpenMainMenu);
                break;
            }
            case EncounterTopic.EndDamageAnimation:
            {
                SetState(State.NegotiatingDamage);

                SelectedOpponent().ReceiveDamage(_attack.Damage);

                if (SelectedOpponent().hitPoints > 0)
                {
                    SetState(State.None);
                    _encounterSubject.Notify(EncounterTopic.OpenMainMenu);
                }
                else
                {
                    var newEvent = new InteractionEvent();
                    newEvent.AddMessage("This opponent is dead");
                    var response = new InteractionResponseEvent(newEvent);
                    _encounterSubject.Notify(response);
                }

                break;
            }
            case EventTopic.EndEventSequence when _state == State.NegotiatingDamage:
            {
                SetState(State.ResolveTurn);

                if (SelectedOpponent().hitPoints < 1)
                {
                    var opponent = SelectedOpponent();
                    _opponents.Remove(opponent);
                    Destroy(opponent.gameObject);
                }

                break;
            }
            case EventTopic.EndEventSequence when _state == State.EndingEncounter:
            {
                _flowSubject.Notify(FlowTopic.EncounterStartWipeOut);

                break;
            }
        }
    }

    private Opponent SelectedOpponent()
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

        SelectedOpponent().GetComponent<Blinker>().shouldBlink = false;

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

        SelectedOpponent().GetComponent<Blinker>().shouldBlink = true;
    }

    // Transition state on next tick to avoid message collision
    private void SetState(State newState)
    {
        _nextState = newState;
    }

    private void Update()
    {
        _state = _nextState;

        if (_state == State.PostAnimationMessage && !messageWindowController.IsVisible())
        {
            SetState(State.ShowDamageValues);
            _damageValue.ShowDamage(_attack.Damage, SelectedOpponent());
        } else if(_state == State.ResolveTurn && !messageWindowController.IsVisible())
        {
            if (_opponents.Count == 0)
            {
                SetState(State.EndingEncounter);
                var newEvent = new InteractionEvent();
                newEvent.AddMessage("All opponents dead");
                var response = new InteractionResponseEvent(newEvent);
                _encounterSubject.Notify(response);
            }
            else
            {
                SetState(State.None);
                _selectedTargetIndex = 0;
                _encounterSubject.Notify(EncounterTopic.OpenMainMenu);
            }
        }
    }

    private void Start()
    {
        foreach (Transform child in _opponentsTransform)
        {
            _opponents.Add(child.GetComponent<Opponent>());
        }
    }
}