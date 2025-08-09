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

    [ShowInEditor, Serialize] private List<Actor> _actorsToDeactivate;

    public override void OnAwake()
    {
        base.OnAwake();
        _jumpscareActor.IsActive = false;
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
        if (_actorsToDeactivate != null)
        {
            foreach (Actor item in _actorsToDeactivate)
            {
                item.IsActive = false;
            }
        }
    }



}
