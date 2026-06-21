using System.Linq;
using Creatures;
using UnityEngine;

[CreateAssetMenu(fileName = "FairyController", menuName = "CreatureAI/Controllers/FairyController")]
public class FairyController : AIController
{
    private float checkInterval;
    private Vector2 targetDirection;

    public override void Init(Creature owner)
    {
        this.owner = owner;
        if (Alignment == Alignment.Ally) creatureLayerMask = LayerMask.GetMask("Player");
        else creatureLayerMask = LayerMask.GetMask("Enemy");

        wallsLayerMask = LayerMask.GetMask("Walls");
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
                .FirstOrDefault(s => s.CanUse(target));

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
                if (target == null) return;
                if ((transform.position - target.transform.position).sqrMagnitude < 4) 
                {
                    Movement.Use(Vector2.zero);
                    return;
                }
                if (Movement.Use(targetDirection.normalized))
                {
                    target = FindTarget();
                }
            }
        }
    }
}