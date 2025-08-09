using System;
using System.Collections.Generic;
using DigiTalino_Plugin;
using FlaxEngine;

namespace Game;

/// <summary>
/// LightSwitch Actor.
/// </summary>
[Category("Interactions")]
public class LightSwitch : AInteraction
{
    [ShowInEditor, Serialize] private Actor _LightActor;
    [ShowInEditor, NonSerialized] private bool _IsOn = true;



    public override void OnAwake()
    {
        base.OnAwake();
        _isToggleable = true;
        ToggleLight(false);
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
        ToggleLight(false);
    }

    /// <inheritdoc/>
    protected override void OnInteract(Actor interactor)
    {
        ToggleLight(!_IsOn);

    }


    private void ToggleLight(bool flag)
    {
        _IsOn = flag;
        _LightActor.IsActive = flag;

    }


}