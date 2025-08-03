using System;
using DigiTalino_Plugin;
using FlaxEngine;
using static DigiTalino_Plugin.Easing;

namespace Game;

/// <summary>
/// ButtonPanel Script.
/// </summary>
public class ButtonPanel : Script
{
    // TODO: Make each button into a script that will handle interaction events but the logic is here. Such as Animations and Sounds
    [ShowInEditor, Serialize] private Actor _elevatorActor;
    [ShowInEditor, Serialize] private Actor _openButton;
    [ShowInEditor, Serialize] private Actor _closeButton;
    [ShowInEditor, Serialize] private Actor _goUpButton;
    [ShowInEditor, Serialize] private Actor _goDownButton;

    [ShowInEditor, Serialize] private SceneAnimation _elevatorOpen;
    [ShowInEditor, Serialize] private SceneAnimation _elevatorClose;
    [ShowInEditor, Serialize] private AudioClip _vibrationSound;
    [ShowInEditor, Serialize] private AudioClip _dingSound;
    [ShowInEditor, Serialize] private AudioClip _moveDoorSound;

    [ShowInEditor, Serialize] private float transitionDuration = 5f;

    // Button Interactions
    private IInteract _openButtonInteract;
    private IInteract _closeButtonInteract;
    private IInteract _goUpButtonInteract;
    private IInteract _goDownButtonInteract;


    // Button Panel State
    private State _state = State.Idle;
    private DoorState _doorState = DoorState.Closed;

    // Elevator Vibration
    private Vector3 _originalPosition;
    private float _vibrationTime = 0f;
    [ShowInEditor, Serialize] private float _vibrationIntensity = 50f; // tweak as needed
    [ShowInEditor, Serialize] private float _vibrationFrequency = 20f; // tweak as needed
    private bool _isVibrating = false;

    [ShowInEditor, Serialize] private TransitionType _easingType = TransitionType.EaseOutSine;

    private float _pendingActionTimer = -1f;
    private Action _pendingActionAfterDoorClose = null;


    public override void OnAwake()
    {
        InitializeButtons();
        SubscribeButtonEvents();
    }

    public override void OnDisable()
    {
        UnsubscribeButtonEvents();
    }

    public override void OnUpdate()
    {
        if (_pendingActionTimer >= 0f)
        {
            _pendingActionTimer -= Time.DeltaTime;
            if (_pendingActionTimer <= 0f)
            {
                _pendingActionTimer = -1f;
                _pendingActionAfterDoorClose?.Invoke();
                _pendingActionAfterDoorClose = null;
            }
        }

        if (_isVibrating && _elevatorActor != null)
        {
            _vibrationTime += Time.DeltaTime;
            float remainingTime = transitionDuration - _vibrationTime;

            float easeFactor = remainingTime <= 1f
                ? Apply(_easingType, Mathf.Clamp(remainingTime, 0f, 1f))
                : 1f;

            float currentIntensity = _vibrationIntensity * easeFactor * 0.001f;
            float currentFrequency = _vibrationFrequency * easeFactor;

            float offsetX = Mathf.Sin(_vibrationTime * currentFrequency) * currentIntensity;
            float offsetY = Mathf.Cos(_vibrationTime * currentFrequency * 0.8f) * currentIntensity;
            float offsetZ = Mathf.Sin(_vibrationTime * currentFrequency * 1.2f) * currentIntensity;

            _elevatorActor.LocalPosition = _originalPosition + new Vector3(offsetX, offsetY, offsetZ);

            if (_vibrationTime >= transitionDuration)
            {

                StopElevatorVibration();
                SwitchToIdle();
                OnOpenButtonInteracted(null);
            }
        }
    }



    private void InitializeButtons()
    {
        _openButtonInteract = GetInteractOrThrow(_openButton, nameof(_openButton));
        _closeButtonInteract = GetInteractOrThrow(_closeButton, nameof(_closeButton));
        _goUpButtonInteract = GetInteractOrThrow(_goUpButton, nameof(_goUpButton));
        _goDownButtonInteract = GetInteractOrThrow(_goDownButton, nameof(_goDownButton));
    }

    private static IInteract GetInteractOrThrow(Actor actor, string name)
    {
        if (actor == null || !actor.TryGetScript(out IInteract interact))
            throw new Exception($"{name} is null or missing IInteract script.");
        return interact;
    }

    private void SubscribeButtonEvents()
    {
        _openButtonInteract.OnInteracted += OnOpenButtonInteracted;
        _closeButtonInteract.OnInteracted += OnCloseButtonInteracted;
        _goUpButtonInteract.OnInteracted += OnGoUpButtonInteracted;
        _goDownButtonInteract.OnInteracted += OnGoDownButtonInteracted;
    }

    private void UnsubscribeButtonEvents()
    {
        _openButtonInteract.OnInteracted -= OnOpenButtonInteracted;
        _closeButtonInteract.OnInteracted -= OnCloseButtonInteracted;
        _goUpButtonInteract.OnInteracted -= OnGoUpButtonInteracted;
        _goDownButtonInteract.OnInteracted -= OnGoDownButtonInteracted;
    }

    private void OnGoUpButtonInteracted(Actor actor)
    {

        if (_state != State.Idle) return;


        if (_doorState == DoorState.Open)
        {
            CloseDoorsThen(() =>
            {
                SwitchState(State.GoingUp);
                StartElevatorVibration();
            });
        }
        else
        {
            SwitchState(State.GoingUp);
            StartElevatorVibration();
        }
    }

    private void OnGoDownButtonInteracted(Actor actor)
    {
        if (_state != State.Idle) return;



        if (_doorState == DoorState.Open)
        {
            CloseDoorsThen(() =>
            {
                SwitchState(State.GoingDown);
                StartElevatorVibration();
            });
        }
        else
        {
            SwitchState(State.GoingDown);
            StartElevatorVibration();
        }
    }

    private void CloseDoorsThen(Action callback)
    {
        CloseDoors();

        _pendingActionAfterDoorClose = callback;
        _pendingActionTimer = transitionDuration;
    }



    private void StartElevatorVibration()
    {

        if (_elevatorActor == null) return;

        _originalPosition = _elevatorActor.LocalPosition;
        _vibrationTime = 0f;
        _isVibrating = true;

        SingletonManager.Get<AudioManager>().Play3DSFXClip(_vibrationSound, _elevatorActor.Position);
    }

    private void StopElevatorVibration()
    {
        if (_elevatorActor == null) return;

        _elevatorActor.LocalPosition = _originalPosition;
        _isVibrating = false;
        SingletonManager.Get<AudioManager>().Play3DSFXClip(_dingSound, _elevatorActor.Position);
    }



    private void OnCloseButtonInteracted(Actor actor)
    {

        if (_state != State.Idle || _doorState == DoorState.Closed) return;

        CloseDoors();
    }

    private void CloseDoors()
    {

        SingletonManager.Get<SceneAnimationManager>().PlayAnimation(_elevatorClose);

        SingletonManager.Get<AudioManager>().Play3DSFXClip(_moveDoorSound, _elevatorActor.Position);


        SwitchState(State.Closing);
        _doorState = DoorState.Closed;
    }


    private void OnOpenButtonInteracted(Actor actor)
    {

        if (_state != State.Idle || _doorState == DoorState.Open) return;

        SingletonManager.Get<SceneAnimationManager>().PlayAnimation(_elevatorOpen);

        SingletonManager.Get<AudioManager>().Play3DSFXClip(_moveDoorSound, _elevatorActor.Position);

        SwitchState(State.Opening);
        _doorState = DoorState.Open;


    }


    private void SwitchState(State newState)
    {
        if (_state != newState)
            _state = newState;
    }

    public void SwitchToIdle()
    {

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
