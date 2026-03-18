using ArrowFlow.Types;
using System.Linq;

public class EffectManager : Singleton<EffectManager>
{
    public Effect[] Effects;

    public void Play(string name)
    {
        Effect effect = Effects.FirstOrDefault(x => x.name == name);
        if(effect == null) return;

        effect.Play();
    }
}