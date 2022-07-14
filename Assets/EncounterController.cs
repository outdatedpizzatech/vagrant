using System.Collections.Generic;
using TMPro;
using UnityEngine;

class Attack
{
    public int Damage;
    public bool IsCritical;

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
    private enum ActionPhase
    {
        InMenu,
        PickingAttackTarget,
        AnnounceAndCalculateAttack,
        DoAttackAnimation,
        AnnounceAttackEffects,
        ShowDamage,
        CommitDamageAndAnnounceDeaths,
        RemoveDeadOpponentsFromPlay,
        ResolveTurn,
        EndingEncounter,
    }

    private enum WhoseTurn
    {
        Player,
        Opponent
    }

    public MessageWindowController messageWindowController;
    public EncounterWindowController encounterWindowController;
    public EncounterCommandWindowController encounterCommandWindowController;
    private Subject _encounterSubject;
    private Subject _flowSubject;
    private ActionPhase _actionPhase;
    private ActionPhase _nextActionPhase;
    private WhoseTurn _whoseTurn;
    private WhoseTurn _nextWhoseTurn;
    private int _selectedTargetIndex;
    private readonly List<Damageable> _opponents = new();
    private AbilityAnimation _abilityAnimation;
    private DamageValue _damageValue;
    private Transform _opponentsTransform;
    private EventStepMarker _eventStepMarker;
    private Attack _attack;
    private PlayerAvatar _playerAvatar;
    private PlayerController _player;
    private TMP_Text _hpText;
    private int _attackingOpponentIndex;
    private bool _playingAnimation;
    private Damageable _targeted;

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
            case EncounterTopic.AttemptingToFlee:
                EndEncounter("Getting out of here...");
                _encounterSubject.Notify(EncounterTopic.CloseMainMenu);
                break;
            case FlowTopic.EncounterFinishedWipeIn:
                SetActionPhase(ActionPhase.InMenu);
                SetWhoseTurn(WhoseTurn.Player);
                break;
            case EncounterTopic.PickedAttack when _whoseTurn == WhoseTurn.Player && _actionPhase == ActionPhase.InMenu:
                SetActionPhase(ActionPhase.PickingAttackTarget);
                break;
            case DirectionalNavigation menuNavigation
                when _actionPhase == ActionPhase.PickingAttackTarget && _whoseTurn == WhoseTurn.Player:
                UpdateTargetSelection(menuNavigation);
                break;
            case EncounterTopic.Cancel
                when _actionPhase == ActionPhase.PickingAttackTarget && _whoseTurn == WhoseTurn.Player:
                SelectedOpponent().GetComponent<Blinker>().shouldBlink = false;
                SetActionPhase(ActionPhase.InMenu);
                break;
            case PlayerRequestsPrimaryActionEvent
                when _actionPhase == ActionPhase.PickingAttackTarget && !_eventStepMarker.Active():
                SetActionPhase(ActionPhase.AnnounceAndCalculateAttack);
                _encounterSubject.Notify(EncounterTopic.CloseMainMenu);
                break;
            case GeneralTopic.PlayerRequestsSecondaryAction:
                _encounterSubject.Notify(EncounterTopic.Cancel);
                break;
            case EncounterTopic.EndAttackAnimation:
            {
                _playingAnimation = false;
                break;
            }
            case EncounterTopic.EndDamageAnimation:
            {
                _playingAnimation = false;
                break;
            }
        }
    }

    private Damageable SelectedOpponent()
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
    private void SetActionPhase(ActionPhase newActionPhase)
    {
        print("setting turn phase to " + newActionPhase);
        _nextActionPhase = newActionPhase;
    }

    // Transition state on next tick to avoid message collision
    private void SetWhoseTurn(WhoseTurn newWhoseTurn)
    {
        print("setting whose turn to " + newWhoseTurn);
        _nextWhoseTurn = newWhoseTurn;
    }

    private void Update()
    {
        _hpText.text = _playerAvatar.GetComponent<Damageable>().hitPoints.ToString();
        _actionPhase = _nextActionPhase;
        _whoseTurn = _nextWhoseTurn;

        if (!encounterWindowController.IsVisible()) return;
        if (_eventStepMarker.Active() || _playingAnimation) return;

        switch (_actionPhase)
        {
            case ActionPhase.InMenu when !encounterCommandWindowController.IsFocused():
                _encounterSubject.Notify(EncounterTopic.OpenMainMenu);
                break;

            case ActionPhase.PickingAttackTarget when _whoseTurn == WhoseTurn.Player:
                SelectedOpponent().GetComponent<Blinker>().shouldBlink = true;
                break;

            case ActionPhase.AnnounceAndCalculateAttack:
            {
                AnnounceAndCalculateAttack();
                break;
            }

            case ActionPhase.DoAttackAnimation:
                DoAttackAnimation();
                break;

            case ActionPhase.AnnounceAttackEffects:
            {
                AnnounceAttackEffects();
                break;
            }

            case ActionPhase.ShowDamage:
                ShowDamage();
                break;

            case ActionPhase.CommitDamageAndAnnounceDeaths:
            {
                CommitDamageAndAnnounceDeaths();

                break;
            }

            case ActionPhase.RemoveDeadOpponentsFromPlay:
            {
                RemoveDeadOpponentsFromPlay();
                break;
            }

            case ActionPhase.ResolveTurn:
            {
                ResolveTurn();
                break;
            }

            case ActionPhase.EndingEncounter:
            {
                _flowSubject.Notify(FlowTopic.EncounterStartWipeOut);

                break;
            }
        }
    }

    private void Start()
    {
        foreach (Transform child in _opponentsTransform)
        {
            _opponents.Add(child.GetComponent<Damageable>());
        }
    }

    private void AnnounceAndCalculateAttack()
    {
        _attack = new Attack();
        _targeted = _whoseTurn == WhoseTurn.Opponent
            ? _playerAvatar.GetComponent<Damageable>()
            : SelectedOpponent();

        var blinker = _targeted.GetComponent<Blinker>();

        if (blinker != null)
        {
            blinker.shouldBlink = false;
        }

        var actor = _whoseTurn == WhoseTurn.Opponent
            ? _opponents[_attackingOpponentIndex].GetComponent<Damageable>()
            : _playerAvatar.GetComponent<Damageable>();

        blinker = actor.GetComponent<Blinker>();

        if (blinker != null)
        {
            blinker.FlashFor(1f);
        }

        AddMessage($"{actor.name} attacks!");
        SetActionPhase(ActionPhase.DoAttackAnimation);
    }

    private void DoAttackAnimation()
    {
        _playingAnimation = true;
        _abilityAnimation.PlaySwordAnimationOn(_targeted.transform);

        SetActionPhase(ActionPhase.AnnounceAttackEffects);
    }

    private void AnnounceAttackEffects()
    {
        SetActionPhase(ActionPhase.ShowDamage);

        if (_attack.IsCritical)
        {
            AddMessage("A critical hit!");
        }
    }

    private void ShowDamage()
    {
        SetActionPhase(ActionPhase.CommitDamageAndAnnounceDeaths);
        _playingAnimation = true;
        _damageValue.ShowDamage(_attack.Damage, _targeted.transform);
    }

    private void CommitDamageAndAnnounceDeaths()
    {
        SetActionPhase(ActionPhase.RemoveDeadOpponentsFromPlay);

        _targeted.ReceiveDamage(_attack.Damage);

        if (_targeted.hitPoints < 1)
        {
            AddMessage($"{_targeted.name} is dead");
        }
    }

    private void ResolveTurn()
    {
        if (_playerAvatar.GetComponent<Damageable>().hitPoints < 1)
        {
            EndEncounter("You have lost the battle.");
            return;
        }

        if (_opponents.Count == 0)
        {
            EndEncounter("All opponents dead");
        }
        
        if (_whoseTurn == WhoseTurn.Player)
        {
            FlipTurn();
            _selectedTargetIndex = 0;
            _attackingOpponentIndex = 0;
        }
        else
        {
            if (_attackingOpponentIndex < _opponents.Count - 1)
            {
                _attackingOpponentIndex++;
                SetActionPhase(ActionPhase.AnnounceAndCalculateAttack);
            }
            else
            {
                FlipTurn();
                _selectedTargetIndex = 0;
                _attackingOpponentIndex = 0;
            }
        }
    }

    private void FlipTurn()
    {
        if (_whoseTurn == WhoseTurn.Player)
        {
            SetWhoseTurn(WhoseTurn.Opponent);
            SetActionPhase(ActionPhase.AnnounceAndCalculateAttack);
        }
        else
        {
            SetWhoseTurn(WhoseTurn.Player);
            SetActionPhase(ActionPhase.InMenu);
        }
    }

    private void EndEncounter(string message)
    {
        SetActionPhase(ActionPhase.EndingEncounter);
        AddMessage(message);
    }

    private void AddMessage(string message)
    {
        var newEvent = new InteractionEvent();
        newEvent.AddMessage(message);
        var response = new InteractionResponseEvent(newEvent);
        _encounterSubject.Notify(response);
    }


    private void RemoveDeadOpponentsFromPlay()
    {
        if (_whoseTurn == WhoseTurn.Player)
        {
            SetActionPhase(ActionPhase.ResolveTurn);

            if (SelectedOpponent().hitPoints < 1)
            {
                var opponent = SelectedOpponent();
                _opponents.Remove(opponent);
                Destroy(opponent.gameObject);
            }
        }
        else
        {
            SetActionPhase(ActionPhase.ResolveTurn);
        }
    }
}