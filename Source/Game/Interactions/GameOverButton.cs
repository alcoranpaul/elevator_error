using System;
using System.Collections.Generic;
using DigiTalino_Plugin;
using FlaxEngine;

namespace Game;

/// <summary>
/// GameOverButton Actor.
/// </summary>
[Category("Interactions")]
public class GameOverButton : AInteraction
{
    /// <inheritdoc/>
    public override void OnAwake()
    {
        base.OnAwake();

    }



    /// <inheritdoc/>
    protected override void OnInteract(Actor interactor)
    {
        Engine.RequestExit();
    }



}