using System;
using UnityEngine;

namespace Creatures
{
    [Serializable]
    public class AIController : ScriptableObject
    {
        protected Creature owner;
        public Creature Owner => owner;

        protected Transform transform => owner.transform;

        protected Creature target;
        public Creature Target => target;

        public Alignment Alignment;
        protected DirectionSkill Movement => owner.BaseMovement;

        public virtual bool IsPlayer => false;

        protected static readonly Collider2D[] targetSearchBuffer = new Collider2D[16];

        protected LayerMask creatureLayerMask;
        protected LayerMask wallsLayerMask;

        public virtual void Init(Creature owner)
        {
            this.owner = owner;

            if (Alignment == Alignment.Ally) creatureLayerMask = LayerMask.GetMask("Enemy");
            else creatureLayerMask = LayerMask.GetMask("Player");
            wallsLayerMask = LayerMask.GetMask("Walls");
        }
        public virtual void UpdateAI() { }

        public virtual Vector3 GetDirectionToTarget()
        {
            if (target == null) return Vector3.zero;
            return (target.transform.position - owner.transform.position).normalized;
        }

        public virtual Vector3 GetTargetPosition()
        {
            if (target == null) return Vector3.zero;
            return target.transform.position;
        }

        public virtual Creature FindTarget()
        {
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, owner.VisionRange, targetSearchBuffer, creatureLayerMask);

            Creature bestTarget = null;
            float bestDist = Mathf.Infinity;
            Vector3 myPos = transform.position;

            for (int i = 0; i < count; i++)
            {
                Collider2D hit = targetSearchBuffer[i];
                
                if (hit.TryGetComponent(out Creature creature))
                {
                    if (creature == owner) continue;
                    if (creature.HealthComponent.IsDead) continue;
                    if (creature.Controller.IsPlayer)
                    {
                        bestTarget = creature;
                        break;
                    }

                    float dist = Vector2.Distance(myPos, creature.transform.position);
                    if (dist >= bestDist) continue;

                    Vector2 dir = (creature.transform.position - myPos).normalized;
                    RaycastHit2D block = Physics2D.Raycast(myPos, dir, dist, wallsLayerMask);
                    
                    if (block.collider == null)
                    {
                        bestDist = dist;
                        bestTarget = creature;
                    }
                }
            }
            Array.Clear(targetSearchBuffer, 0, count); 

            return bestTarget;
        }
    }
}