using System;
using System.Collections.Generic;
using DigiTalino_Plugin;
using FlaxEngine;

namespace Game;

/// <summary>
/// DirtyItem Actor.
/// </summary>
[Category("Interactions")]
public class DirtyItem : AInteraction
{

    /// <inheritdoc/>
    protected override void OnInteract(Actor interactor)
    {
        Destroy(Actor.Parent);
    }
}