// DigiTalino

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game;

/// <summary>
/// AnomalyData class.
/// </summary>
public class AnomalyData
{
    public AnomalyType type;
    public AnomalyTrigger trigger;
    public bool isTriggered; // track if it's been activated
}
