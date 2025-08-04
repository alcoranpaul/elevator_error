using System;
using System.Collections.Generic;
using DigiTalino_Plugin;
using FlaxEngine;

namespace Game;

/// <summary>
/// Manages floor progression, floor states, and anomaly handling in the game.
/// Handles floor changes, cleaning status, and anomaly assignment.
/// </summary>
[Category("Manager")]
public class FloorManager : InstanceManagerScript
{
    /// <summary>
    /// Event invoked when the current floor changes.
    /// Passes the new floor index (zero-based).
    /// </summary>
    public event Action<int> OnFloorChanged;

    /// <summary>
    /// Reference to the button panel script responsible for floor change input.
    /// </summary>
    [ShowInEditor, Serialize] private ButtonPanel _buttonPanel;

    /// <summary>
    /// Array holding data for each floor.
    /// </summary>
    private FloorData[] _floors;

    /// <summary>
    /// The floor data currently active.
    /// </summary>
    private FloorData _currentFloor;

    /// <summary>
    /// Total number of floors in the building.
    /// Consider moving this to engine settings later.
    /// </summary>
    private const int FLOOR_COUNT = 5;

    /// <inheritdoc/>
    public override void OnAwake()
    {
        base.OnAwake();
        _floors = new FloorData[FLOOR_COUNT];
        InitializeFloors();
        _currentFloor = null;

        _buttonPanel.OnFloorAdvanceRequested += OnFloorAdvanceRequested;
    }

    /// <inheritdoc/>
    public override void OnDisable()
    {
        _buttonPanel.OnFloorAdvanceRequested -= OnFloorAdvanceRequested;
        base.OnDisable();
    }

    /// <summary>
    /// Handler called when the elevator requests to advance to the next floor.
    /// Checks if the current floor is cleaned before allowing progression.
    /// </summary>
    /// <returns>True if the floor was changed; false if the current floor is not cleaned.</returns>
    private bool OnFloorAdvanceRequested()
    {
        if (IsCurrentFloorCleaned())
        {
            GoToNextFloor();
            OnFloorChanged?.Invoke(FindFloorNumber(_currentFloor) + 1);
            return true;
        }
        else
        {
            Debug.Log("Floor is not cleaned!");
            return false;
        }
    }

    /// <summary>
    /// Initializes the floors and assigns random anomalies to some floors.
    /// </summary>
    private void InitializeFloors()
    {
        for (int i = 0; i < FLOOR_COUNT; i++)
        {
            FloorData floor = new();

            // 40% chance to add an anomaly to this floor
            if (Random.Shared.NextDouble() < 0.4)
            {
                AnomalyData anomaly = new AnomalyData()
                {
                    type = GetRandomVisualAnomaly(),
                    trigger = GetRandomTrigger(),
                    isTriggered = false
                };
                floor.Anomalies = [anomaly];
            }

            _floors[i] = floor;
        }
    }

    /// <summary>
    /// Moves the current floor pointer to the next floor.
    /// If no current floor is set, it starts at the first floor.
    /// </summary>
    private void GoToNextFloor()
    {
        if (_currentFloor == null)
        {
            _currentFloor = _floors[0];
        }
        else
        {
            int currentFloorNumber = FindFloorNumber(_currentFloor);
            _currentFloor = _floors[currentFloorNumber + 1];
        }
    }

    /// <summary>
    /// Finds the index of the specified floor data in the floors array.
    /// </summary>
    /// <param name="floorData">Floor data to find.</param>
    /// <returns>Index of the floor in the array or -1 if not found.</returns>
    private int FindFloorNumber(FloorData floorData)
    {
        if (_floors == null || _floors.Length == 0)
            Debug.LogError("Floors array is null or empty");

        for (int i = 0; i < _floors.Length; i++)
        {
            if (_floors[i] == floorData)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Selects a random visual anomaly type for a floor.
    /// </summary>
    /// <returns>A random visual anomaly type.</returns>
    private AnomalyType GetRandomVisualAnomaly()
    {
        AnomalyType[] types = new[]
        {
                AnomalyType.Visual_Disappear,
                AnomalyType.Visual_Misaligned
            };
        return types[Random.Shared.Next(0, types.Length)];
    }

    /// <summary>
    /// Selects a random trigger type for an anomaly.
    /// </summary>
    /// <returns>A random anomaly trigger type.</returns>
    private AnomalyTrigger GetRandomTrigger()
    {
        AnomalyTrigger[] triggers =
        [
                AnomalyTrigger.OnFloorEnter,
                AnomalyTrigger.OnRoomEnter,
                AnomalyTrigger.OnDelay
            ];
        return triggers[Random.Shared.Next(0, triggers.Length)];
    }

    /// <summary>
    /// Returns whether the current floor has been fully cleaned.
    /// If no current floor is set, returns true by default.
    /// </summary>
    /// <returns>True if the current floor is cleaned or no floor set; otherwise false.</returns>
    public bool IsCurrentFloorCleaned()
    {
        if (_currentFloor == null)
            return true;
        return _currentFloor.IsCleaned;
    }
}