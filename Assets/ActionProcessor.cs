using System.Collections.Generic;
using UnityEngine;

public class ActionProcessor : MonoBehaviour, IObserver
{
    private enum ActionPhase
    {
        Idle,
        DoAbilityAnimation,
        AnnounceAbilityEffects,
        ShowDamage,
        CommitDamageAndAnnounceDeaths,
        RemoveDeadOpponentsFromPlay,
    }
    
    private readonly List<Attack> _attacks = new();
    private ActionPhase _nextActionPhase;
    private ActionPhase _actionPhase = ActionPhase.Idle;
    private EncounterTargeter _targeter;
    private AbilityAnimation _abilityAnimationPrefab;
    private EncounterWindowController _encounterWindowController;
    private Subject _encounterSubject;
    private Ability _pickedAbility;
    private int _currentTargetIndex;
    private int _activeAnimationCount;
    private DamageValue _damageValue;
    private EventStepMarker _eventStepMarker;

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
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
            case EncounterEvents.BeginAction beginAction:
            {
                BeginAction(beginAction.Actor, beginAction.Ability);
                break;
            }
        }
    }

    public void Setup(EncounterTargeter targeter, AbilityAnimation abilityAnimationPrefab, Subject encounterSubject,
        EncounterWindowController encounterWindowController, DamageValue damageValue, EventStepMarker eventStepMarker)
    {
        _targeter = targeter;
        _abilityAnimationPrefab = abilityAnimationPrefab;
        _encounterSubject = encounterSubject;
        _encounterWindowController = encounterWindowController;
        _damageValue = damageValue;
        _eventStepMarker = eventStepMarker;

        _encounterSubject.AddObserver(this);
    }

    public bool Active()
    {
        return (_actionPhase != ActionPhase.Idle);
    }

    private void Update()
    {
        _actionPhase = _nextActionPhase;

        if (!_encounterWindowController.IsVisible()) return;
        if (_eventStepMarker.Active() || _activeAnimationCount > 0) return;

        switch (_actionPhase)
        {
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
        }
    }

    private void SetActionPhase(ActionPhase newActionPhase)
    {
        _nextActionPhase = newActionPhase;
    }

    private void DoAbilityAnimation()
    {
        _targeter.SelectedTargets().ForEach(x =>
        {
            var abilityAnimation = GameObject.Instantiate(_abilityAnimationPrefab, Vector2.zero, Quaternion.identity)
                .GetComponent<AbilityAnimation>();
            abilityAnimation.transform.parent = _encounterWindowController.transform;
            abilityAnimation.Setup(_encounterSubject);
            abilityAnimation.PlayAnimation(x.transform, _pickedAbility.animationName);
            _activeAnimationCount++;
        });

        SetActionPhase(ActionPhase.AnnounceAbilityEffects);
    }
    
    private void BeginAction(Damageable actor, Ability pickedAbility)
    {
        _attacks.Clear();
        _currentTargetIndex = 0;
        _pickedAbility = pickedAbility;
        _targeter.SelectedTargets().ForEach((_) => { _attacks.Add(new Attack()); });

        SetActionPhase(ActionPhase.DoAbilityAnimation);
        AddMessage($"{actor.name} uses {_pickedAbility.name} on {_targeter.TargetName()}!");
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
        _damageValue.ShowDamage(_attacks[_currentTargetIndex].Damage,
            _targeter.SelectedTarget(_currentTargetIndex).transform);
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

    private void AddMessage(string message)
    {
        _encounterSubject.Notify(new EncounterEvents.EncounterMessage(message));
    }

    private void RemoveDeadOpponentsFromPlay()
    {
        var selectedTarget = _targeter.SelectedTarget(_currentTargetIndex);

        if (selectedTarget.GetComponent<PartyAvatar>() == null)
        {
            if (selectedTarget.hitPoints < 1)
            {
                selectedTarget.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }

        if (_targeter.AtEndOfTargetList(_currentTargetIndex))
        {
            SetActionPhase(ActionPhase.Idle);
        }
        else
        {
            SetActionPhase(ActionPhase.AnnounceAbilityEffects);
            _currentTargetIndex++;
        }
    }
}