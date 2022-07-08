using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EncounterController : MonoBehaviour, IObserver
{
    private enum State
    {
        None,
        PickingAttackTarget,
    }

    private Subject _encounterSubject;
    private State _state;
    private int _selectedTargetIndex;
    private List<Blinker> _opponents = new();

    private void Start()
    {
        foreach (Transform child in GameObject.Find("WorldSpaceCanvas/EncounterBox/Opponents").transform)
        {
            _opponents.Add(child.GetComponent<Blinker>());
        }
    }

    public void Setup(Subject encounterSubject)
    {
        _encounterSubject = encounterSubject;
        _encounterSubject.AddObserver(this);
    }

    public void OnNotify(SubjectMessage message)
    {
        if (message == SubjectMessage.PickedAttack)
        {
            _state = State.PickingAttackTarget;
            _opponents[_selectedTargetIndex].shouldBlink = true;
        }
        else if (_state == State.PickingAttackTarget && message == SubjectMessage.Cancel)
        {
            _opponents[_selectedTargetIndex].shouldBlink = false;
            GameObject.Find("WorldSpaceCanvas/EncounterBox/Cragman").GetComponent<Blinker>().shouldBlink = false;
            _state = State.None;
            _encounterSubject.Notify(SubjectMessage.OpenMainMenu);
        }
    }

    public void OnNotify<T>(T parameters)
    {
        print(_state);
        print(parameters);
        switch (parameters)
        {
            case MenuNavigation menuNavigation when _state == State.PickingAttackTarget:
                UpdateTargetSelection(menuNavigation);
                break;
        }
    }

    private void UpdateTargetSelection(MenuNavigation menuNavigation)
    {
        var targetCount = _opponents.Count;

        if (targetCount == 0)
        {
            return;
        }

        _opponents[_selectedTargetIndex].shouldBlink = false;

        print("advancing... " + menuNavigation.Direction);

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

        _opponents[_selectedTargetIndex].shouldBlink = true;
    }
}