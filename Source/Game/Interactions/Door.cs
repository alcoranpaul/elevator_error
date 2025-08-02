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
        base.OnAwake();
        SwitchState(State.Close, false);
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

    private void SwitchState(State newState, bool playAnimation = true)
    {
        if (_state == newState)
            return;

        _state = newState;

        if (!playAnimation) return;
        SceneAnimationManager sceneManager = SingletonManager.Get<SceneAnimationManager>();
        AudioManager audioManager = SingletonManager.Get<AudioManager>();

        switch (_state)
        {
            case State.Open:
                sceneManager.PlayAnimation(DOOR_NAME, _doorActor, _openDoor, OnAnimationFinished);
                audioManager.Play3DSFXClip(_openDoorSFX, _doorActor.Position);
                break;
            case State.Close:
                sceneManager.PlayAnimation(DOOR_NAME, _doorActor, _closeDoor, OnAnimationFinished);
                audioManager.Play3DSFXClip(_closeDoorSFX, _doorActor.Position);
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