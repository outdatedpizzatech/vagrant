using System.Collections.Generic;
using UnityEngine;

public class EncounterController : MonoBehaviour, IObserver
{
    private enum ActionPhase
    {
        PickingAction,
        PickingAbilityTarget,
        AnnounceAndCalculateAttack,
        DoAbilityAnimation,
        AnnounceAbilityEffects,
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
    private int _targetedPartyTargetIndex;
    private readonly List<Damageable> _opponents = new();
    private readonly List<Damageable> _partyMembers = new();
    private DamageValue _damageValue;
    private Transform _opponentsTransform;
    private EventStepMarker _eventStepMarker;
    private List<Attack> _attacks = new();
    private PlayerController _player;
    private int _attackingOpponentIndex;
    private int _activePartyMemberIndex;
    private int _activeAnimationCount;
    private HpBox _hpBox;
    private int _partyMemberCount;
    private Ability _pickedAbility;
    private Ability _roundhouse = new Ability("Roundhouse", "SwordSlash", Ability.TargetingMode.Single);
    private List<Damageable> _allParticipants = new();
    private Material _blinkMaterial;
    private int _currentTargetIndex;
    public AbilityAnimation abilityAnimationPrefab;
    private EncounterTargeter _targeter;
    private AbilityProcessor _abilityProcessor;

    private void Awake()
    {
        _hpBox = GameObject.Find("WorldSpaceCanvas/EncounterWindow/HPBox").GetComponent<HpBox>();
        _blinkMaterial = Resources.Load<Material>("Materials/Blink");
        _targeter = new EncounterTargeter(_opponents, _partyMembers);
        _abilityProcessor = new AbilityProcessor();
    }

    public void Setup(Subject encounterSubject, Transform opponentsTransform,
        Subject interactionSubject, DamageValue damageValue, Subject flowSubject)
    {
        _encounterSubject = encounterSubject;
        _encounterSubject.AddObserver(this);
        _opponentsTransform = opponentsTransform;
        _eventStepMarker = new EventStepMarker(encounterSubject, messageWindowController, interactionSubject);
        interactionSubject.AddObserver(this);
        _damageValue = damageValue;
        _damageValue.Setup(_encounterSubject);
        _flowSubject = flowSubject;
        _flowSubject.AddObserver(this);
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case FlowTopic.StartEncounter:
                SetupField();
                break;
            case EncounterTopic.AttemptingToFlee:
                EndEncounter("Getting out of here...");
                _encounterSubject.Notify(EncounterTopic.CloseMainMenu);
                break;
            case FlowTopic.EncounterFinishedWipeIn:
                SetActionPhase(ActionPhase.PickingAction);
                SetWhoseTurn(WhoseTurn.Player);
                break;
            case PickedAbility pickedAbility
                when _whoseTurn == WhoseTurn.Player && _actionPhase == ActionPhase.PickingAction:
                SetActionPhase(ActionPhase.PickingAbilityTarget);
                _pickedAbility = pickedAbility.Ability;
                _targeter.PickDefaultTargetForPlayer(_pickedAbility.targetingMode);
                break;
            case DirectionalNavigation menuNavigation
                when _actionPhase == ActionPhase.PickingAbilityTarget && _whoseTurn == WhoseTurn.Player:
                _targeter.UpdateTargetSelection(menuNavigation, _pickedAbility.targetingMode);
                break;
            case EncounterTopic.Cancel
                when _actionPhase == ActionPhase.PickingAbilityTarget && _whoseTurn == WhoseTurn.Player:
                _targeter.ClearSelection();
                SetActionPhase(ActionPhase.PickingAction);
                break;
            case PlayerRequestsPrimaryActionEvent
                when _actionPhase == ActionPhase.PickingAbilityTarget && !_eventStepMarker.Active():
                SetActionPhase(ActionPhase.AnnounceAndCalculateAttack);
                _encounterSubject.Notify(EncounterTopic.CloseMainMenu);
                break;
            case GeneralTopic.PlayerRequestsSecondaryAction:
                _encounterSubject.Notify(EncounterTopic.Cancel);
                break;
            case EncounterTopic.EndAttackAnimation:
            {
                _activeAnimationCount--;
                break;
            }
            case EncounterTopic.EndDamageAnimation:
            {
                _activeAnimationCount--;
                break;
            }
        }
    }

    private Damageable ActivePartyMember()
    {
        return _partyMembers[_activePartyMemberIndex];
    }


    // Transition state on next tick to avoid message collision
    private void SetActionPhase(ActionPhase newActionPhase)
    {
        print("setting action phase to " + newActionPhase);
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
        _actionPhase = _nextActionPhase;
        _whoseTurn = _nextWhoseTurn;

        if (!encounterWindowController.IsVisible()) return;
        if (_eventStepMarker.Active() || _activeAnimationCount > 0) return;

        switch (_actionPhase)
        {
            case ActionPhase.PickingAction
                when _whoseTurn == WhoseTurn.Player && !encounterCommandWindowController.IsFocused():
                _targeter.ClearSelection();
                if (ActivePartyMember().hitPoints > 0)
                {
                    _encounterSubject.Notify(new OpenEncounterCommandWindow(ActivePartyMember()));
                }
                else
                {
                    SetActionPhase(ActionPhase.ResolveTurn);
                }

                break;
            case ActionPhase.PickingAction
                when _whoseTurn == WhoseTurn.Opponent:
                _targeter.ClearSelection();
                if (_opponents[_attackingOpponentIndex].hitPoints > 0)
                {
                    _pickedAbility = _roundhouse;
                    SetActionPhase(ActionPhase.PickingAbilityTarget);
                }
                else
                {
                    SetActionPhase(ActionPhase.ResolveTurn);
                }

                break;

            case ActionPhase.PickingAbilityTarget when _whoseTurn == WhoseTurn.Player:
                _targeter.BlinkSelectedTargets();
                break;

            case ActionPhase.PickingAbilityTarget when _whoseTurn == WhoseTurn.Opponent:
                _targeter.PickTargetForOpponent();
                SetActionPhase(ActionPhase.AnnounceAndCalculateAttack);
                break;

            case ActionPhase.AnnounceAndCalculateAttack:
            {
                AnnounceAndCalculateAttack();
                break;
            }

            case ActionPhase.DoAbilityAnimation:
                DoAbilityAnimation();
                break;

            case ActionPhase.AnnounceAbilityEffects:
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

    private void AnnounceAndCalculateAttack()
    {
        _currentTargetIndex = 0;
        _targeter.SelectedTargets().ForEach((_) => { _attacks.Add(new Attack()); });
        _targeter.BlinkSelectedTargets(false);

        var actor = _whoseTurn == WhoseTurn.Opponent
            ? _opponents[_attackingOpponentIndex].GetComponent<Damageable>()
            : ActivePartyMember();

        if (_whoseTurn == WhoseTurn.Opponent)
        {
            var blinker = actor.GetComponent<Blinker>();

            if (blinker != null)
            {
                blinker.FlashFor(1f);
            }
        }

        AddMessage($"{actor.name} uses {_pickedAbility.name} on {_targeter.TargetName()}!");
        SetActionPhase(ActionPhase.DoAbilityAnimation);
    }

    private void DoAbilityAnimation()
    {
        _abilityProcessor.DoAbilityAnimation(_targeter, ref _activeAnimationCount, abilityAnimationPrefab, _pickedAbility, _encounterSubject, encounterWindowController);

        SetActionPhase(ActionPhase.AnnounceAbilityEffects);
    }

    private void AnnounceAttackEffects()
    {
        SetActionPhase(ActionPhase.ShowDamage);

        if (_attacks[_currentTargetIndex].IsCritical)
        {
            AddMessage("A critical hit!");
        }
    }

    private void ShowDamage()
    {
        SetActionPhase(ActionPhase.CommitDamageAndAnnounceDeaths);
        _activeAnimationCount++;
        _damageValue.ShowDamage(_attacks[_currentTargetIndex].Damage, _targeter.SelectedTarget(_currentTargetIndex).transform);
    }

    private void CommitDamageAndAnnounceDeaths()
    {
        SetActionPhase(ActionPhase.RemoveDeadOpponentsFromPlay);

        var targeted = _targeter.SelectedTarget(_currentTargetIndex);

        targeted.ReceiveDamage(_attacks[_currentTargetIndex].Damage);

        if (targeted.hitPoints < 1)
        {
            AddMessage($"{targeted.name} is dead");
        }
    }

    private void ResolveTurn()
    {
        if (_partyMembers.Find(x => x.hitPoints > 0) == null)
        {
            EndEncounter("You have lost the battle.");
            return;
        }

        if (!_opponents.Find(x => x.hitPoints > 0))
        {
            EndEncounter("All opponents dead");
            return;
        }

        if (_whoseTurn == WhoseTurn.Player)
        {
            if (_activePartyMemberIndex < _partyMemberCount - 1)
            {
                _activePartyMemberIndex++;
                SetActionPhase(ActionPhase.PickingAction);
            }
            else
            {
                FlipTurn();
            }
        }
        else
        {
            if (_attackingOpponentIndex < _opponents.Count - 1)
            {
                _attackingOpponentIndex++;
                SetActionPhase(ActionPhase.PickingAction);
            }
            else
            {
                FlipTurn();
            }
        }
    }

    private void ResetIndexes()
    {
        _attacks.Clear();
        _targetedPartyTargetIndex = 0;
        _attackingOpponentIndex = 0;
        _activePartyMemberIndex = 0;
    }

    private void FlipTurn()
    {
        if (_whoseTurn == WhoseTurn.Player)
        {
            SetWhoseTurn(WhoseTurn.Opponent);
            SetActionPhase(ActionPhase.PickingAction);
        }
        else
        {
            SetWhoseTurn(WhoseTurn.Player);
            SetActionPhase(ActionPhase.PickingAction);
        }

        ResetIndexes();
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
            var opponent = _targeter.SelectedTarget(_currentTargetIndex);
            if (opponent.hitPoints < 1)
            {
                opponent.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }

        if (_targeter.AtEndOfTargetList(_currentTargetIndex))
        {
            SetActionPhase(ActionPhase.ResolveTurn);
        }
        else
        {
            SetActionPhase(ActionPhase.AnnounceAbilityEffects);
            _currentTargetIndex++;
        }
    }

    private void SetupField()
    {
        Transform partyContainer = encounterWindowController.transform.Find("Party");

        var sourceTransform = GameObject.Find("/Party").transform;

        const float inc = 1f;
        _partyMemberCount = sourceTransform.childCount;
        var x = (_partyMemberCount - 1) * inc * -1;

        foreach (Transform source in sourceTransform)
        {
            var copy = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
            copy.transform.parent = partyContainer;

            copy.name = source.name;

            var spriteRenderer = copy.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = source.GetComponent<SpriteRenderer>().sprite;
            spriteRenderer.sortingOrder = 150;

            var rectTransform = copy.AddComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = new Vector2(x, -3.5f);

            var animator = copy.AddComponent<Animator>();
            animator.runtimeAnimatorController = source.GetComponent<Animator>().runtimeAnimatorController;

            var damageable = copy.AddComponent<Damageable>();
            damageable.hitPoints = 50;

            copy.AddComponent<PartyAvatar>();
            var blinker = copy.AddComponent<Blinker>();
            blinker.blinkMaterial = _blinkMaterial;

            _hpBox.AddDamageable(damageable);

            x += inc * 2;

            _partyMembers.Add(damageable);

            _allParticipants.Add(damageable);
        }

        foreach (Transform child in _opponentsTransform)
        {
            _opponents.Add(child.GetComponent<Damageable>());
        }
    }
}