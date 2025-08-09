// DigiTalino

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game;

/// <summary>
/// TVAnomaly Script.
/// </summary>
public class TVAnomaly : Anomaly
{
    [ShowInEditor, Serialize] private VideoPlayer _videoPlayer;

    public override void OnAwake()
    {
        base.OnAwake();
    }

    public override void Activate()
    {
        base.Activate();
        _videoPlayer.Play();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        _videoPlayer.Stop();
    }

}
