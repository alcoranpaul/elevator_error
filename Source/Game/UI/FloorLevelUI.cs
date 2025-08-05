
using System;
using DigiTalino_Plugin;
using FlaxEngine;
using FlaxEngine.GUI;

namespace Game;

public class FloorLevelUI : Script
{
    [ShowInEditor, Serialize] private UIControl _labelControl;
    [ShowInEditor, Serialize] private ButtonPanel _buttonPanel;

    private Label _floorLabel;

    private int _floorLevel;

    private const string GROUND_FLOOR = "Ground";

    public override void OnStart()
    {
        _floorLabel = UIHelper.GetLabel(_labelControl);
        _floorLabel.Text = GROUND_FLOOR;
        SingletonManager.Get<FloorManager>().OnFloorChangeRequested += OnFloorChanged;
        _buttonPanel.OnElevatorStoppedVibrating += OnButtonPanel_OnElevatorStopped;
    }


    public override void OnDisable()
    {
        SingletonManager.Get<FloorManager>().OnFloorChangeRequested -= OnFloorChanged;
        _buttonPanel.OnElevatorStoppedVibrating -= OnButtonPanel_OnElevatorStopped;
        base.OnDisable();
    }

    private void OnFloorChanged(int obj)
    {
        if (obj <= 0) Debug.LogError("Floor number should be greater than 0");


        _floorLevel = obj;

    }

    private void OnButtonPanel_OnElevatorStopped()
    {
        if (_floorLevel == 0)
            _floorLabel.Text = GROUND_FLOOR;
        else
            _floorLabel.Text = $"{_floorLevel}";
    }

}