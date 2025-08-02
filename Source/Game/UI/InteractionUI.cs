using System;
using System.Collections.Generic;
using DigiTalino_Plugin;

using FlaxEngine;
using FlaxEngine.GUI;

namespace Game;

/// <summary>
/// InteractionUI Script.
/// </summary>
public class InteractionUI : Script
{
    [ShowInEditor, Serialize] private UIControl _contentControl;
    [ShowInEditor, Serialize] private UIControl _interactionTextControl;

    private Label _interactionText;


    public override void OnAwake()
    {
        _interactionText = UIHelper.GetLabel(_interactionTextControl);
        _contentControl.Control.Visible = false;
    }

    public override void OnStart()
    {
        SingletonManager.Get<InteractionComponent>().OnFocusChanged += OnFocusChanged;
    }

    private void OnFocusChanged(string obj)
    {
        _contentControl.Control.Visible = !string.IsNullOrEmpty(obj);
        _interactionText.Text = obj;
    }

}
