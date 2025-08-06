using System;
using System.Collections.Generic;
using DigiTalino_Plugin;
using FlaxEngine;
using FlaxEngine.GUI;

namespace Game;

/// <summary>
/// MessageManager Manager. This class follows the singleton pattern!
/// </summary>
[Category("Manager")]
public class MessageManager : InstanceManagerScript
{
    [ShowInEditor, Serialize] private UIControl _messageContainer;
    [ShowInEditor, Serialize] private UIControl _messageControl;

    [ShowInEditor, Serialize] private SceneAnimation _messageAnimation;

    private Label _messageLabel;
    /// <inheritdoc/>
    public override void OnAwake()
    {
        base.OnAwake(); // Do not remove since it is required

        _messageLabel = UIHelper.GetLabel(_messageControl);

        _messageContainer.Control.Visible = false;

    }

    public void ShowMessage(string message)
    {
        _messageLabel.Text = message;
        SingletonManager.Get<SceneAnimationManager>().PlayAnimation(_messageAnimation);
    }





}