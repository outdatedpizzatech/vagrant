using System.Collections.Generic;
using UnityEngine;

public class EncounterTargeter
{
    private bool _targetingOpponents;
    private int _selectedTargetIndex;
    private readonly List<Damageable> _opponents;
    private readonly List<Damageable> _partyMembers;
    private readonly List<Damageable> _selectedTargets = new();

    public EncounterTargeter(List<Damageable> opponents, List<Damageable> partyMembers)
    {
        _opponents = opponents;
        _partyMembers = partyMembers;
    }

    public void UpdateTargetSelection(DirectionalNavigation directionalNavigation, Ability.TargetingMode targetingMode)
    {
        switch (directionalNavigation.Direction)
        {
            case Enums.Direction.Up or Enums.Direction.Down:
                _targetingOpponents = !_targetingOpponents;
                _selectedTargetIndex = 0;
                break;
        }

        var targetList = _targetingOpponents ? _opponents : _partyMembers;
        var targetCount = targetList.Count;

        void IncrementTarget(int inc)
        {
            _selectedTargetIndex += inc;
            _selectedTargetIndex = _selectedTargetIndex < 0 ? targetCount - 1 : _selectedTargetIndex % targetCount;
        }

        while (targetList[_selectedTargetIndex].hitPoints < 1)
        {
            IncrementTarget(1);
        }

        _selectedTargets.ForEach(x => x.GetComponent<Blinker>().shouldBlink = false);
        _selectedTargets.Clear();

        switch (directionalNavigation.Direction)
        {
            case Enums.Direction.Right:
                IncrementTarget(1);

                while (targetList[_selectedTargetIndex].hitPoints < 1)
                {
                    IncrementTarget(1);
                }

                break;
            case Enums.Direction.Left:
                IncrementTarget(-1);

                while (targetList[_selectedTargetIndex].hitPoints < 1)
                {
                    IncrementTarget(-1);
                }

                break;
        }

        if (targetingMode == Ability.TargetingMode.Single)
        {
            _selectedTargets.Add(targetList[_selectedTargetIndex]);
        }
        else
        {
            targetList.ForEach(x => _selectedTargets.Add(x));
        }
    }

    public void PickDefaultTargetForPlayer(Ability.TargetingMode targetingMode)
    {
        if (targetingMode == Ability.TargetingMode.Single)
        {
            var index = 0;

            while (_opponents[index].hitPoints < 1)
            {
                index++;
            }

            _selectedTargetIndex = index;
            _selectedTargets.Add(_opponents[index]);
        }
        else
        {
            _opponents.ForEach(x =>
            {
                if (x.hitPoints > 0) _selectedTargets.Add(x);
            });
        }

        _targetingOpponents = true;
    }

    public void PickTargetForOpponent()
    {
        var partyMemberCount = _partyMembers.Count;

        _selectedTargetIndex = Random.Range(0, partyMemberCount);

        while (_partyMembers[_selectedTargetIndex].hitPoints < 1)
        {
            _selectedTargetIndex = Random.Range(0, partyMemberCount);
        }

        _selectedTargets.Add(_partyMembers[_selectedTargetIndex]);
    }

    public List<Damageable> SelectedTargets()
    {
        return _selectedTargets;
    }

    public void ClearSelection()
    {
        _selectedTargets.ForEach(x => x.GetComponent<Blinker>().shouldBlink = false);
        _selectedTargets.Clear();
    }


    public void BlinkSelectedTargets(bool on)
    {
        _selectedTargets.ForEach(x => x.GetComponent<Blinker>().shouldBlink = on);
    }

    public void BlinkSelectedTargets()
    {
        BlinkSelectedTargets(true);
    }

    public string TargetName()
    {
        var targetName = _selectedTargets.Count == 1
            ? _selectedTargets[0].name
            : (_targetingOpponents ? "the enemies" : "the party");

        return targetName;
    }

    public Damageable SelectedTarget(int index)
    {
        return _selectedTargets[index];
    }

    public bool AtEndOfTargetList(int index)
    {
        return index >= _selectedTargets.Count - 1;
    }
}