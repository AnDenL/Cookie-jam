using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(menuName = "CreatureAI/Skills/HealAlly")]
    public class HealAllySkill : AllyTargetedSkill
    {
        public float amount;

        public override SkillType Type => SkillType.Utility;

        public override void Activate(Creature target)
        {
            if (target == null) return;

            target.HealthComponent.Heal(amount);

            ParticleManager.PlayParticle("Heal", target.transform.position, (int)amount);
        }
    }
}