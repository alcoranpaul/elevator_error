using System;
using System.Collections.Generic;
using DigiTalino_Plugin;
using FlaxEngine;
using FlaxEngine.Utilities;

namespace Game;

/// <summary>
/// Door Actor.
/// </summary>
[Category("Interactions")]
public class Door : AInteraction
{
    [ShowInEditor, Serialize] private SceneAnimation _openDoor;
    [ShowInEditor, Serialize] private SceneAnimation _closeDoor;
    [ShowInEditor, Serialize] private Actor _doorActor;

    private State _state;

    private bool _isTurning = false;

    private const string DOOR_NAME = "Door";

    /// <inheritdoc/>
    public override void OnAwake()
    {
        base.OnAwake();
        _state = State.Close;
    }


    /// <inheritdoc/>
    protected override void OnInteract(Actor interactor)
    {
        ToggleDoor();
    }

    private void ToggleDoor()
    {
        if (_isTurning) return;

        switch (_state)
        {
            case State.Open:
                SwitchState(State.Close);
                break;
            case State.Close:
                SwitchState(State.Open);
                break;
        }
    }

    private void SwitchState(State newState)
    {
        if (_state == newState)
            return;

        _state = newState;

        switch (_state)
        {
            case State.Open:
                SingletonManager.Get<SceneAnimationManager>().PlayAnimation(DOOR_NAME, _doorActor, _openDoor, OnAnimationFinished);
                break;
            case State.Close:
                SingletonManager.Get<SceneAnimationManager>().PlayAnimation(DOOR_NAME, _doorActor, _closeDoor, OnAnimationFinished);
                break;
        }

        _isTurning = true;
    }

    private void OnAnimationFinished(Actor actor)
    {
        _isTurning = false;
        FinishInteraction();
    }

    private enum State
    {
        Open,
        Close
    }


}