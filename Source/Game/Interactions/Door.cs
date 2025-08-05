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

    [ShowInEditor, Serialize] private AudioClip _openDoorSFX, _closeDoorSFX;



    private State _state;

    private bool _isTurning = false;

    private const string DOOR_NAME = "Door";

    /// <inheritdoc/>
    public override void OnAwake()
    {
        _isToggleable = true;
        base.OnAwake();
        SwitchState(State.Close, false, false);
    }

    public override void OnStart()
    {
        SingletonManager.Get<ButtonPanel>().OnElevatorStoppedVibrating += OnFloorChangeRequested;
    }

    public override void OnDisable()
    {
        SingletonManager.Get<ButtonPanel>().OnElevatorStoppedVibrating -= OnFloorChangeRequested;
        base.OnDisable();
    }

    private void OnFloorChangeRequested()
    {
        SwitchState(State.Close, true, false);
    }

    /// <inheritdoc/>
    protected override void OnInteract(Actor interactor)
    {
        Debug.Log("Door interacted");
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

    private void SwitchState(State newState, bool playAnimation = true, bool playSound = true)
    {
        if (_state == newState)
            return;

        Debug.Log($"Door state changed to {newState}");

        _state = newState;


        SceneAnimationManager sceneManager = SingletonManager.Get<SceneAnimationManager>();
        AudioManager audioManager = SingletonManager.Get<AudioManager>();

        switch (_state)
        {
            case State.Open:
                if (playAnimation)
                    sceneManager.PlayAnimation(DOOR_NAME, _doorActor, _openDoor, OnAnimationFinished);
                if (playSound)
                    audioManager.Play3DSFXClip(_openDoorSFX, _doorActor.Position);
                break;
            case State.Close:
                if (playAnimation)
                    sceneManager.PlayAnimation(DOOR_NAME, _doorActor, _closeDoor, OnAnimationFinished);
                if (playSound)
                    audioManager.Play3DSFXClip(_closeDoorSFX, _doorActor.Position);
                break;
        }
        if (playAnimation)
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