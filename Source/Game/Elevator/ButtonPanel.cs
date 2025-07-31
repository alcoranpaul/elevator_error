using System;
using System.Collections.Generic;
using DigiTalino_Plugin;
using FlaxEngine;
using FlaxEngine.Utilities;

namespace Game;

/// <summary>
/// ButtonPanel Script.
/// </summary>
public class ButtonPanel : Script
{
    [ShowInEditor, Serialize] private Actor _openButton;
    [ShowInEditor, Serialize] private Actor _closeButton;
    [ShowInEditor, Serialize] private Actor _goUpButton;
    [ShowInEditor, Serialize] private Actor _goDownButton;

    [ShowInEditor, Serialize] private SceneAnimation _elevatorOpen;
    [ShowInEditor, Serialize] private SceneAnimation _elevatorClose;
    [ShowInEditor, Serialize] private SceneAnimation _buttonPress;


    private IInteract _openButtonInteract;
    private IInteract _closeButtonInteract;
    private IInteract _goUpButtonInteract;
    private IInteract _goDownButtonInteract;

    private State _state;
    private DoorState _doorState;

    public override void OnAwake()
    {
        InitializeButtons();

        _state = State.Idle;
        _doorState = DoorState.Closed;
        _openButtonInteract.OnInteracted += OnOpenButtonInteracted;
        _closeButtonInteract.OnInteracted += OnCloseButtonInteracted;
        _goUpButtonInteract.OnInteracted += OnGoUpButtonInteracted;
        _goDownButtonInteract.OnInteracted += OnGoDownButtonInteracted;

    }

    public override void OnDisable()
    {
        _openButtonInteract.OnInteracted -= OnOpenButtonInteracted;
        _closeButtonInteract.OnInteracted -= OnCloseButtonInteracted;
        _goUpButtonInteract.OnInteracted -= OnGoUpButtonInteracted;
        _goDownButtonInteract.OnInteracted -= OnGoDownButtonInteracted;
    }



    private void OnGoUpButtonInteracted(Actor actor)
    {
        if (_state != State.Idle) return;
        throw new NotImplementedException();
        SwitchState(State.Opening);
        PlayButtonAnimation(_goUpButton);
    }


    private void OnGoDownButtonInteracted(Actor actor)
    {
        if (_state != State.Idle) return;
        throw new NotImplementedException();
        SwitchState(State.Closing);
        PlayButtonAnimation(_goDownButton);
    }

    private void OnCloseButtonInteracted(Actor actor)
    {
        Debug.Log($"State: {_state} -- DoorState: {_doorState}");
        if (_state != State.Idle || _doorState == DoorState.Closed) return;
        SingletonManager.Get<SceneAnimationManager>().PlayAnimation(_elevatorClose);
        _closeButton.Layer = 0;
        SwitchState(State.Closing);
        _doorState = DoorState.Closed;
        PlayButtonAnimation(_closeButton);

    }

    private void OnOpenButtonInteracted(Actor actor)
    {
        Debug.Log($"State: {_state} -- DoorState: {_doorState}");
        if (_state != State.Idle || _doorState == DoorState.Open) return;
        SingletonManager.Get<SceneAnimationManager>().PlayAnimation(_elevatorOpen);
        _openButton.Layer = 0;
        SwitchState(State.Opening);
        _doorState = DoorState.Open;
        PlayButtonAnimation(_openButton);

    }

    private void PlayButtonAnimation(Actor buttonActor)
    {
        Actor parent = buttonActor.Parent;
        SingletonManager.Get<SceneAnimationManager>().PlayAnimation("Button", parent, _buttonPress);
    }


    private void InitializeButtons()
    {
        if (_openButton == null || !_openButton.TryGetScript(out _openButtonInteract))
            throw new Exception("Open button is null");

        if (_closeButton == null || !_closeButton.TryGetScript(out _closeButtonInteract))
            throw new Exception("Close button is null");

        if (_goUpButton == null || !_goUpButton.TryGetScript(out _goUpButtonInteract))
            throw new Exception("Go Up button is null");

        if (_goDownButton == null || !_goDownButton.TryGetScript(out _goDownButtonInteract))
            throw new Exception("Go Down button is null");
    }

    private void SwitchState(State newState)
    {
        if (newState == _state) return;
        _state = newState;
    }

    public void SwitchToIdle()
    {

        _closeButton.Layer = 2;
        _openButton.Layer = 2;
        SwitchState(State.Idle);
    }

    private enum State
    {
        Idle,
        Opening,
        Closing,
        GoingUp,
        GoingDown
    }

    private enum DoorState
    {
        Open,
        Closed
    }
}
