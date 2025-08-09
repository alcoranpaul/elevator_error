// DigiTalino

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game;

/// <summary>
/// TVAnomaly Script.
/// </summary>
public class JumpscareAnomaly : Anomaly
{
    [ShowInEditor, Serialize] private Collider _collider;
    [ShowInEditor, Serialize] private Actor _jumpscareActor;

    public override void OnAwake()
    {
        base.OnAwake();
        _collider.IsTrigger = true;
        _collider.TriggerEnter += OnTriggerEnter;
    }

    public override void OnDisable()
    {
        _collider.TriggerEnter -= OnTriggerEnter;
        base.OnDisable();
    }
    private void OnTriggerEnter(PhysicsColliderActor actor)
    {
        _collider.TriggerEnter -= OnTriggerEnter;
        _jumpscareActor.IsActive = true;
    }



}
