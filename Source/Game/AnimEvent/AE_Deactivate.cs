using FlaxEngine;

#if FLAX_EDITOR
using FlaxEditor;
#endif

namespace Game;

public class AE_Deactivate : AnimEvent
{
    public override void OnEvent(AnimatedModel actor, Animation anim, float time, float deltaTime)
    {

#if FLAX_EDITOR
        if (!Editor.IsPlayMode) return;
#endif
        if (actor == null) return;

        actor.IsActive = false;
    }
}