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
    [ShowInEditor, Serialize] private Actor _elevatorActor;
    [ShowInEditor, Serialize] private Actor _openButton;
    [ShowInEditor, Serialize] private Actor _closeButton;
    [ShowInEditor, Serialize] private Actor _goUpButton;
    [ShowInEditor, Serialize] private Actor _goDownButton;

    [ShowInEditor, Serialize] private SceneAnimation _elevatorOpen;
    [ShowInEditor, Serialize] private SceneAnimation _elevatorClose;
    [ShowInEditor, Serialize] private SceneAnimation _buttonPress;

    [ShowInEditor, Serialize] private AudioClip _buttonPressSound;

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
        if (_isVibrating && _elevatorActor != null)
        {
            _vibrationTime += Time.DeltaTime;
            float remainingTime = transitionDuration - _vibrationTime;

            // Determine easing factor only in the last second
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

        _goUpButton.Layer = 0;
        PlayButtonAnimation(_goUpButton);
        SwitchState(State.GoingUp);
        StartElevatorVibration();
    }

    private void OnGoDownButtonInteracted(Actor actor)
    {
        if (_state != State.Idle) return;

        _goDownButton.Layer = 0;
        PlayButtonAnimation(_goDownButton);
        SwitchState(State.GoingDown);
        StartElevatorVibration();
    }

    private void StartElevatorVibration()
    {
        if (_elevatorActor == null) return;

        _originalPosition = _elevatorActor.LocalPosition;
        _vibrationTime = 0f;
        _isVibrating = true;
    }

    private void StopElevatorVibration()
    {
        if (_elevatorActor == null) return;

        _elevatorActor.LocalPosition = _originalPosition;
        _isVibrating = false;
    }



    private void OnCloseButtonInteracted(Actor actor)
    {

        if (_state != State.Idle || _doorState == DoorState.Closed) return;

        SingletonManager.Get<SceneAnimationManager>().PlayAnimation(_elevatorClose);
        _closeButton.Layer = 0;
        PlayButtonAnimation(_closeButton);
        SwitchState(State.Closing);
        _doorState = DoorState.Closed;
    }

    private void OnOpenButtonInteracted(Actor actor)
    {

        if (_state != State.Idle || _doorState == DoorState.Open) return;

        SingletonManager.Get<SceneAnimationManager>().PlayAnimation(_elevatorOpen);
        _openButton.Layer = 0;
        PlayButtonAnimation(_openButton);
        SwitchState(State.Opening);
        _doorState = DoorState.Open;
    }

    private void PlayButtonAnimation(Actor buttonActor)
    {
        if (buttonActor?.Parent == null) return;

        SingletonManager.Get<SceneAnimationManager>()
            .PlayAnimation("Button", buttonActor.Parent, _buttonPress);

        SingletonManager.Get<AudioManager>().Play3DSFXClip(_buttonPressSound, buttonActor.Position, 0.5f);

    }

    private void SwitchState(State newState)
    {
        if (_state != newState)
            _state = newState;
    }

    public void SwitchToIdle()
    {
        _openButton.Layer = 2;
        _closeButton.Layer = 2;
        _goUpButton.Layer = 2;
        _goDownButton.Layer = 2;
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
