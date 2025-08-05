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
    private bool _IsOn = true;



    public override void OnAwake()
    {
        base.OnAwake();
        ToggleLight();
        _isToggleable = true;
    }

    /// <inheritdoc/>
    protected override void OnInteract(Actor interactor)
    {
        ToggleLight();

    }


    private void ToggleLight()
    {
        _IsOn = !_IsOn;
        _LightActor.IsActive = _IsOn;
        Debug.Log("Toggle Light: " + _IsOn);
    }


}