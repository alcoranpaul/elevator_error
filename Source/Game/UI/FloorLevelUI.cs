
using System;
using DigiTalino_Plugin;
using FlaxEngine;
using FlaxEngine.GUI;

namespace Game;

public class FloorLevelUI : Script
{
    [ShowInEditor, Serialize] private UIControl _labelControl;

    private Label _floorLabel;

    public override void OnStart()
    {
        _floorLabel = UIHelper.GetLabel(_labelControl);
        _floorLabel.Text = "G";
        SingletonManager.Get<FloorManager>().OnFloorChanged += OnFloorChanged;
    }

    public override void OnDisable()
    {
        SingletonManager.Get<FloorManager>().OnFloorChanged -= OnFloorChanged;
        base.OnDisable();
    }

    private void OnFloorChanged(int obj)
    {
        if (obj <= 0) Debug.LogError("Floor number should be greater than 0");

        _floorLabel.Text = $"{obj}";
    }
}