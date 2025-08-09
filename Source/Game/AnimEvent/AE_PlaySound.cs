using DigiTalino_Plugin;
using FlaxEngine;

#if FLAX_EDITOR
using FlaxEditor;
#endif

namespace Game;

public class AE_PlaySound : AnimEvent
{
    public AudioClip Sound;
    public CameraShakeEvent ShakeEvent;
    public override void OnEvent(AnimatedModel actor, Animation anim, float time, float deltaTime)
    {

#if FLAX_EDITOR
        if (!Editor.IsPlayMode) return;
#endif
        if (actor == null) return;

        SingletonManager.Get<AudioManager>().PlaySFXClip(Sound);
        SingletonManager.Get<EventManager>().Publish(ShakeEvent);
    }
}