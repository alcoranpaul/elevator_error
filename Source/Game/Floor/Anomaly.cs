// DigiTalino

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game;

/// <summary>
/// Anomaly Script.
/// </summary>
public class Anomaly : Script, IAnomaly
{

    /// <inheritdoc/>
    public override void OnAwake()
    {
        Deactivate();
    }



    public virtual void Activate()
    {
        Actor.IsActive = true;

    }

    public virtual void Deactivate()
    {
        Actor.IsActive = false;

    }
}
