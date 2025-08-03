// DigiTalino

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game;

/// <summary>
/// AnomalyType class.
/// </summary>
public enum AnomalyTrigger
{
    OnFloorEnter,
    OnRoomEnter,
    OnDelay,
    Manual // For scripted or future use
}
