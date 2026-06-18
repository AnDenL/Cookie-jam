using Creatures;
using UnityEngine;

[CreateAssetMenu(menuName = "CreatureAI/Skills/ApplyEffectSkill")]
public class ApplyEffectSkill : TargetedSkill
{
    public Effect Effect;
    public string Particles;

    public override SkillType Type => SkillType.Utility;

    public override void Activate(Creature target)
    {
        if (target == null) return;

        target.AddEffect(Effect);

        ParticleManager.PlayParticle(Particles, target.transform.position, 1);
    }
}