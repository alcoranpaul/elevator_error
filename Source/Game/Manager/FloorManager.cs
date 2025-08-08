using System;
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
    public event Action<int> OnFloorChangeRequested;

    /// <summary>
    /// Reference to the button panel script responsible for floor change input.
    /// </summary>
    [ShowInEditor, Serialize] private ButtonPanel _buttonPanel;

    [ShowInEditor, Serialize] private SkyLight _skyLight;

    /// <summary>
    /// Array holding data for each floor.
    /// </summary>
    private FloorData[] _floors;

    /// <summary>
    /// The floor data currently active.
    /// </summary>
    private FloorData _currentFloor;

    public int CurrentFloor => FindFloorNumber(_currentFloor) + 1;

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
        GoToGroundFloor();
        _skyLight.Brightness = 3f;
        _buttonPanel.OnFloorAdvanceRequested += OnFloorAdvanceRequested;
        _buttonPanel.OnElevatorStoppedVibrating += OnElevatorStoppedVibrating;
    }

    private void OnElevatorStoppedVibrating()
    {
        if (_currentFloor == null)
            _skyLight.Brightness = 3f;
        else
            _skyLight.Brightness = 0.7f;
    }

    public override void OnStart()
    {
        SingletonManager.Get<MessManager>().OnFloorCleaned += OnFloorCleaned;
    }


    /// <inheritdoc/>
    public override void OnDisable()
    {
        _buttonPanel.OnFloorAdvanceRequested -= OnFloorAdvanceRequested;
        SingletonManager.Get<MessManager>().OnFloorCleaned -= OnFloorCleaned;
        _buttonPanel.OnElevatorStoppedVibrating -= OnElevatorStoppedVibrating;
        base.OnDisable();
    }

    /// <summary>
    /// Initializes the floors and assigns random anomalies to some floors.
    /// </summary>
    private void InitializeFloors()
    {
        for (int i = 0; i < FLOOR_COUNT; i++)
        {
            FloorData floor = new();
            _floors[i] = floor;
        }
    }

    /// <summary>
    /// Handles elevator request to advance to the next floor.
    /// Checks if the current floor is cleaned before allowing movement.
    /// </summary>
    /// <param name="direction">Requested direction of movement.</param>
    /// <returns>True if the floor was changed; false if not cleaned.</returns>
    private bool OnFloorAdvanceRequested(ButtonPanel.Direction direction)
    {
        if (_currentFloor == null) // Ground floor start
        {
            GoToNextFloor();
            OnFloorChangeRequested?.Invoke(1);
            return true;
        }

        if (!IsCurrentFloorCleaned())
        {
            Debug.Log("Floor is not cleaned!");
            SingletonManager.Get<MessageManager>().ShowMessage("Floor is not cleaned!");
            return false;
        }

        bool hasAnomaly = _currentFloor.HasAnomaly;
        Debug.Log($"Floor has anomaly: {hasAnomaly}, Direction: {direction}");

        bool isGoingDown = direction == ButtonPanel.Direction.Down;
        bool isGoingUp = direction == ButtonPanel.Direction.Up;

        // Logic to decide floor movement
        if ((!hasAnomaly && isGoingDown) || (hasAnomaly && isGoingUp))
        {
            GoToGroundFloor();
            OnFloorChangeRequested?.Invoke(-1);
        }
        else if ((hasAnomaly && isGoingDown) || (!hasAnomaly && isGoingUp))
        {
            GoToNextFloor();

            OnFloorChangeRequested?.Invoke(FindFloorNumber(_currentFloor) + 1);
        }

        return true;
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
            return;
        }

        int currentIndex = FindFloorNumber(_currentFloor);
        int nextIndex = currentIndex + 1;

        if (nextIndex >= _floors.Length)
        {
            Debug.Log("All floors have been cleaned!");
            return;
        }

        _currentFloor = _floors[nextIndex];
        _currentFloor.IsCleaned = false;
        _currentFloor.HasAnomaly = false;


    }

    /// <summary>
    /// Moves the current floor pointer to the first floor.
    /// </summary>
    private void GoToGroundFloor()
    {
        _currentFloor = null;

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

    private void OnFloorCleaned()
    {
        _currentFloor.IsCleaned = true;
    }

}