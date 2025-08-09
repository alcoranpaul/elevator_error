// DigiTalino

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game;

/// <summary>
/// Anomaly Script.
/// </summary>
public abstract class Anomaly : Script, IAnomaly
{

    /// <inheritdoc/>
    public override void OnAwake()
    {
        Deactivate();
    }



    public virtual void Activate()
    {
        Actor.IsActive = true;
        Debug.Log($"Anomaly Activated!: {Actor.IsActive}");
    }

    public virtual void Deactivate()
    {
        Actor.IsActive = false;
        Debug.Log($"Anomaly Deactivated!: {Actor.IsActive}");
    }
}
