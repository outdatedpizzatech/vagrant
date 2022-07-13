using System.Collections.Generic;
using TMPro;
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

    public Attack()
    {
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
        PlayerPickingAttackTarget,
        PlayerStartingAttack,
        PlayerInAttackAnimation,
        PlayerPostAnimationMessage,
        PlayerShowDamageValues,
        PlayerNegotiatingDamage,
        PlayerResolveTurn,
        EndingEncounter,
        EnemyPickingAttackTarget,
        EnemyStartingAttack,
        EnemyInAttackAnimation,
        EnemyPostAnimationMessage,
        EnemyShowDamageValues,
        EnemyNegotiatingDamage,
        EnemyResolveTurn,
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
    private PlayerAvatar _playerAvatar;
    private TMP_Text _hpText;
    private int _attackingOpponentIndex;

    private void Awake()
    {
        _playerAvatar = GameObject.Find("WorldSpaceCanvas/EncounterWindow/PlayerAvatar").GetComponent<PlayerAvatar>();
        _hpText = GameObject.Find("WorldSpaceCanvas/EncounterWindow/HPBox/Text").GetComponent<TMP_Text>();
    }

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
            case DirectionalNavigation menuNavigation when _state == State.PlayerPickingAttackTarget:
                UpdateTargetSelection(menuNavigation);
                break;
            case EncounterTopic.PickedAttack:
                SetState(State.PlayerPickingAttackTarget);
                SelectedOpponent().GetComponent<Blinker>().shouldBlink = true;
                break;
            case EncounterTopic.Cancel when _state == State.PlayerPickingAttackTarget:
                SelectedOpponent().GetComponent<Blinker>().shouldBlink = false;
                SetState(State.None);
                _encounterSubject.Notify(EncounterTopic.OpenMainMenu);
                break;
            case PlayerRequestsPrimaryActionEvent
                when _state == State.PlayerPickingAttackTarget && !_eventStepMarker.Active():
                _encounterSubject.Notify(EncounterTopic.AttackTarget);
                break;
            case GeneralTopic.PlayerRequestsSecondaryAction:
                _encounterSubject.Notify(EncounterTopic.Cancel);
                break;
            case EncounterTopic.AttackTarget:
            {
                _attack = new Attack(SelectedOpponent());
                SetState(State.PlayerStartingAttack);
                var newEvent = new InteractionEvent();
                newEvent.AddMessage("Player attacks!");
                var response = new InteractionResponseEvent(newEvent);
                _encounterSubject.Notify(response);
                var selectedOpponent = SelectedOpponent();
                selectedOpponent.GetComponent<Blinker>().shouldBlink = false;
                break;
            }
            case EventTopic.EndEventSequence when _state == State.PlayerStartingAttack:
            {
                var selectedOpponent = SelectedOpponent();
                _abilityAnimation.PlaySwordAnimationOn(selectedOpponent.transform);

                SetState(State.PlayerInAttackAnimation);
                break;
            }
            case EncounterTopic.EndAttackAnimation when _state == State.PlayerInAttackAnimation:
            {
                SetState(State.PlayerPostAnimationMessage);
                if (_attack.IsCritical)
                {
                    var newEvent = new InteractionEvent();
                    newEvent.AddMessage("A critical hit!");
                    var response = new InteractionResponseEvent(newEvent);
                    _encounterSubject.Notify(response);
                }

                break;
            }
            case EncounterTopic.EndAttackAnimation when _state == State.EnemyInAttackAnimation:
            {
                SetState(State.EnemyPostAnimationMessage);
                if (_attack.IsCritical)
                {
                    var newEvent = new InteractionEvent();
                    newEvent.AddMessage("A critical hit!");
                    var response = new InteractionResponseEvent(newEvent);
                    _encounterSubject.Notify(response);
                }

                break;
            }
            case EncounterTopic.EndDamageAnimation when _state == State.PlayerShowDamageValues:
            {
                SetState(State.PlayerNegotiatingDamage);

                SelectedOpponent().ReceiveDamage(_attack.Damage);

                if (SelectedOpponent().hitPoints > 0)
                {
                    SetState(State.EnemyPickingAttackTarget);
                    _encounterSubject.Notify(EncounterTopic.CloseMainMenu);
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
            case EncounterTopic.EndDamageAnimation when _state == State.EnemyShowDamageValues:
            {
                SetState(State.EnemyNegotiatingDamage);

                _playerAvatar.ReceiveDamage(_attack.Damage);

                if (_playerAvatar.hitPoints < 1)
                {
                    var newEvent = new InteractionEvent();
                    newEvent.AddMessage("Player is dead");
                    var response = new InteractionResponseEvent(newEvent);
                    _encounterSubject.Notify(response);
                }

                break;
            }
            case EventTopic.EndEventSequence when _state == State.PlayerNegotiatingDamage:
            {
                SetState(State.PlayerResolveTurn);

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
        print("switching state to " + newState);
        _nextState = newState;
    }

    private void Update()
    {
        _hpText.text = _playerAvatar.hitPoints.ToString();
        _state = _nextState;

        if (_state == State.EnemyPostAnimationMessage && !messageWindowController.IsVisible())
        {
            SetState(State.EnemyShowDamageValues);
            _damageValue.ShowDamage(_attack.Damage, _playerAvatar.transform);
        }
        else if (_state == State.EnemyNegotiatingDamage && !messageWindowController.IsVisible())
        {
            SetState(State.EnemyResolveTurn);
        }
        else if (_state == State.PlayerPostAnimationMessage && !messageWindowController.IsVisible())
        {
            SetState(State.PlayerShowDamageValues);
            _damageValue.ShowDamage(_attack.Damage, SelectedOpponent().transform);
        }
        else if (_state == State.PlayerResolveTurn && !messageWindowController.IsVisible())
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
                SetState(State.EnemyPickingAttackTarget);
                _selectedTargetIndex = 0;
                _attackingOpponentIndex = 0;
            }
        }
        else if (_state == State.EnemyResolveTurn && !messageWindowController.IsVisible())
        {
            if (_playerAvatar.hitPoints < 1)
            {
                SetState(State.EndingEncounter);
                var newEvent = new InteractionEvent();
                newEvent.AddMessage("You have lost the battle.");
                var response = new InteractionResponseEvent(newEvent);
                _encounterSubject.Notify(response);
            }
            else
            {
                if (_attackingOpponentIndex < _opponents.Count - 1)
                {
                    _attackingOpponentIndex++;
                    SetState(State.EnemyPickingAttackTarget);
                }
                else
                {
                    SetState(State.None);
                    _selectedTargetIndex = 0;
                    _attackingOpponentIndex = 0;
                    _encounterSubject.Notify(EncounterTopic.OpenMainMenu);
                }
            }
        }
        else if (_state == State.EnemyPickingAttackTarget && !messageWindowController.IsVisible())
        {
            _attack = new Attack();
            SetState(State.EnemyStartingAttack);
            var newEvent = new InteractionEvent();
            newEvent.AddMessage($"{_opponents[_attackingOpponentIndex].gameObject.name} attacks!");
            var response = new InteractionResponseEvent(newEvent);
            _opponents[_attackingOpponentIndex].GetComponent<Blinker>().FlashFor(1f);
            _encounterSubject.Notify(response);
        }
        else if (_state == State.EnemyStartingAttack && !messageWindowController.IsVisible())
        {
            _abilityAnimation.PlaySwordAnimationOn(_playerAvatar.transform);

            SetState(State.EnemyInAttackAnimation);
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