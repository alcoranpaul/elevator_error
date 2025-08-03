using System;
using System.Collections.Generic;
using DigiTalino_Plugin;
using FlaxEngine;

namespace Game;

/// <summary>
/// AElevatorButton Script.
/// </summary>
[Category("Interactions")]
public class ElevatorButton : AInteraction
{
    [ShowInEditor, Serialize] private AudioClip _buttonPressSound;
    [ShowInEditor, Serialize] private SceneAnimation _buttonPressAnim;
    /// <inheritdoc/>
    public override void OnAwake()
    {
        base.OnAwake();

    }

    /// <inheritdoc/>
    public override void OnStart()
    {
        base.OnStart();

    }

    /// <inheritdoc/>
    protected override void OnInteract(Actor interactor)
    {
        PlayButtonAnimation();
    }

    private void PlayButtonAnimation()
    {

        SingletonManager.Get<SceneAnimationManager>()
            .PlayAnimation("Button", Actor.Parent, _buttonPressAnim, (actor) => { FinishInteraction(); });

        SingletonManager.Get<AudioManager>().Play3DSFXClip(_buttonPressSound, Actor.Parent.Position, 0.5f);

    }


}
