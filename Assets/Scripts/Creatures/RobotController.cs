using UnityEngine;
using System.Linq;

namespace Creatures
{
    [CreateAssetMenu(fileName = "RobotController", menuName = "CreatureAI/Controllers/RobotController")]
    public class RobotController : AIController
    {
        private float checkTime;

        public override void Init(Creature owner)
        {
            base.Init(owner);

            checkTime = Random.Range(0f, 1f);
        }

        public override void UpdateAI()
        {
            if (owner.HealthComponent.IsDead) return;

            if (Time.time > checkTime)
            {
                target = owner.FindTarget();
                checkTime = Time.time + 1f;
            }

            if (target != null)
            {
                float dist = Vector2.Distance(owner.transform.position, target.transform.position);
                owner.LookAt(target.transform.position);

                foreach (var skill in owner.ActiveSkills.OfType<TargetedSkill>())
                {
                    if (skill.CanUse(target))
                        skill.Use(target);
                }
                if (dist < 3f)
                {
                    foreach (var escape in owner.ActiveSkills
                        .Where(s => s.Type == SkillType.Movement || s.Type == SkillType.Defense))
                    {
                        Vector2 dir = (owner.transform.position - target.transform.position).normalized;
                        escape.Use(owner.transform.position + (Vector3)dir);
                    }

                    Movement.Use(-GetDirectionToTarget());
                }
                else if (dist > 7f)
                {
                    Movement.Use(GetDirectionToTarget());
                }
            }
        }
    }
}
