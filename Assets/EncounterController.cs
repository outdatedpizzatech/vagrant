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
    private int _selectedTargetIndex;
    private int _targetedPartyTargetIndex;
    private readonly List<Damageable> _opponents = new();
    private readonly List<Damageable> _partyMembers = new();
    private AbilityAnimation _abilityAnimation;
    private DamageValue _damageValue;
    private Transform _opponentsTransform;
    private EventStepMarker _eventStepMarker;
    private Attack _attack;
    private PlayerController _player;
    private int _attackingOpponentIndex;
    private int _activePartyMemberIndex;
    private bool _playingAnimation;
    private Damageable _targeted;
    private HpBox _hpBox;
    private int _partyMemberCount;
    private Ability _pickedAbility;
    private Ability _roundhouse = new Ability("Roundhouse", "SwordSlash", Ability.TargetingMode.Single);
    private List<Damageable> _selectedTargets = new();
    private List<Damageable> _allParticipants = new();
    private bool _targetingOpponents;
    private Material _blinkMaterial;

    private void Awake()
    {
        _hpBox = GameObject.Find("WorldSpaceCanvas/EncounterWindow/HPBox").GetComponent<HpBox>();
        _blinkMaterial = Resources.Load<Material>("Materials/Blink");
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
                break;
            case DirectionalNavigation menuNavigation
                when _actionPhase == ActionPhase.PickingAbilityTarget && _whoseTurn == WhoseTurn.Player:
                UpdateTargetSelection(menuNavigation);
                break;
            case EncounterTopic.Cancel
                when _actionPhase == ActionPhase.PickingAbilityTarget && _whoseTurn == WhoseTurn.Player:
                _selectedTargets.ForEach(x => x.GetComponent<Blinker>().shouldBlink = false);
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

    private List<Damageable> SelectedTargets()
    {
        return _selectedTargets;
    }

    private Damageable ActivePartyMember()
    {
        return _partyMembers[_activePartyMemberIndex];
    }

    private void UpdateTargetSelection(DirectionalNavigation directionalNavigation)
    {
        switch (directionalNavigation.Direction)
        {
            case Enums.Direction.Up:
                _targetingOpponents = !_targetingOpponents;
                _selectedTargetIndex = 0;
                break;
            case Enums.Direction.Down:
                _targetingOpponents = !_targetingOpponents;
                _selectedTargetIndex = 0;
                break;
        }
        
        var targetList = _targetingOpponents ? _opponents : _partyMembers;
        var targetCount = targetList.Count;

        if (targetCount == 0)
        {
            return;
        }

        _selectedTargets.ForEach(x => x.GetComponent<Blinker>().shouldBlink = false);
        _selectedTargets.Clear();

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

        _selectedTargets.Add(targetList[_selectedTargetIndex]);
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
        _actionPhase = _nextActionPhase;
        _whoseTurn = _nextWhoseTurn;

        if (!encounterWindowController.IsVisible()) return;
        if (_eventStepMarker.Active() || _playingAnimation) return;

        switch (_actionPhase)
        {
            case ActionPhase.PickingAction
                when _whoseTurn == WhoseTurn.Player && !encounterCommandWindowController.IsFocused():
                if (ActivePartyMember().hitPoints > 0)
                {
                    _selectedTargets.Clear();
                    _selectedTargets.Add(_opponents[0]);
                    _targetingOpponents = true;
                    _encounterSubject.Notify(new OpenEncounterCommandWindow(ActivePartyMember()));
                }
                else
                {
                    SetActionPhase(ActionPhase.ResolveTurn);
                }

                break;
            case ActionPhase.PickingAction
                when _whoseTurn == WhoseTurn.Opponent:
                _pickedAbility = _roundhouse;
                SetActionPhase(ActionPhase.PickingAbilityTarget);

                break;

            case ActionPhase.PickingAbilityTarget when _whoseTurn == WhoseTurn.Player:
                _selectedTargets.ForEach(x => x.GetComponent<Blinker>().shouldBlink = true);
                break;

            case ActionPhase.PickingAbilityTarget when _whoseTurn == WhoseTurn.Opponent:
                _selectedTargetIndex = Random.Range(0, _partyMemberCount);

                while (_partyMembers[_selectedTargetIndex].hitPoints < 1)
                {
                    _selectedTargetIndex = Random.Range(0, _partyMemberCount);
                }
                _selectedTargets.Add(_partyMembers[_selectedTargetIndex]);

                SetActionPhase(ActionPhase.AnnounceAndCalculateAttack);
                break;

            case ActionPhase.AnnounceAndCalculateAttack:
            {
                AnnounceAndCalculateAttack();
                break;
            }

            case ActionPhase.DoAbilityAnimation:
                DoAttackAnimation();
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
        _attack = new Attack();
        _targeted = SelectedTargets()[0];

        var blinker = _targeted.GetComponent<Blinker>();

        if (blinker != null)
        {
            blinker.shouldBlink = false;
        }

        var actor = _whoseTurn == WhoseTurn.Opponent
            ? _opponents[_attackingOpponentIndex].GetComponent<Damageable>()
            : ActivePartyMember();

        blinker = actor.GetComponent<Blinker>();

        if (blinker != null)
        {
            blinker.FlashFor(1f);
        }

        AddMessage($"{actor.name} uses {_pickedAbility.name} on {_targeted.name}!");
        SetActionPhase(ActionPhase.DoAbilityAnimation);
    }

    private void DoAttackAnimation()
    {
        _playingAnimation = true;
        _abilityAnimation.PlayAnimation(_targeted.transform, _pickedAbility.animationName);

        SetActionPhase(ActionPhase.AnnounceAbilityEffects);
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
        if (_partyMembers.Find(x => x.hitPoints > 0) == null)
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
                SetActionPhase(ActionPhase.PickingAbilityTarget);
            }
            else
            {
                FlipTurn();
            }
        }
    }

    private void ResetIndexes()
    {
        _selectedTargetIndex = 0;
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
            SetActionPhase(ActionPhase.ResolveTurn);

            var opponent = SelectedTargets()[0];
            if (opponent.hitPoints >= 1) return;
            _opponents.Remove(opponent);
            Destroy(opponent.gameObject);
        }
        else
        {
            SetActionPhase(ActionPhase.ResolveTurn);
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