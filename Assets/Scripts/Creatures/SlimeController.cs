using UnityEngine;
using System.Linq;

namespace Creatures
{
    [CreateAssetMenu(fileName = "SlimeController", menuName = "CreatureAI/Controllers/SlimeController")]
    public class SlimeController : AIController
    {
        private float checkInterval;
        private Vector2 targetDirection;

        public override void Init(Creature owner)
        {
            base.Init(owner);
            checkInterval = Random.Range(0, 0.5f);
        }

        public override Vector3 GetDirectionToTarget()
        {
            if (target == null) return Random.insideUnitCircle;
            return (target.transform.position - owner.transform.position).normalized;
        }

        public override void UpdateAI()
        {
            if (checkInterval < Time.time)
            {
                target = FindTarget();
                checkInterval = Time.time + 0.5f;

                targetDirection = GetDirectionToTarget();
                
                owner.LookAt(owner.transform.position + (Vector3)targetDirection);
                
                Skill chosen = owner.ActiveSkills
                    .OrderByDescending(s => s.Priority)
                    .FirstOrDefault(s => s.CanUse(targetDirection));

                if (chosen != null)
                {
                    switch (chosen)
                    {
                        case TargetedSkill targeted:
                            targeted.Use(target);
                            break;
                        case PositionSkill pos:
                            pos.Use(target.transform.position);
                            break;
                        case DirectionSkill dir:
                            dir.Use(targetDirection);
                            break;
                        case SelfSkill self:
                            self.Use();
                            break;
                    }
                }
            }
            else
            {
                if (Movement != null && targetDirection.magnitude != 0)
                {
                    owner.LookAt(owner.transform.position + (Vector3)targetDirection);
                    if (Movement.Use(targetDirection.normalized))
                    {
                        target = FindTarget();
                    }
                }
            }
        }
    }
}
