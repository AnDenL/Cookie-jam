using System.Collections;
using Creatures;
using UnityEngine;

[CreateAssetMenu(fileName = "Kus", menuName = "CreatureAI/Skills/Kus")]
public class Kus : PositionSkill
{
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly WaitForSeconds _waitForSeconds0_2 = new(0.2f);
    public float damage;
    public float radius;
    public string effect;
    public LayerMask layer;

    public override void Activate(Vector2 position)
    {
        owner.Animator.SetTrigger(AttackHash);
        owner.Cast(MakeKus(position));
        ParticleManager.PlayParticle(effect, position, 1);
    }

    public IEnumerator MakeKus(Vector2 position)
    {
        yield return _waitForSeconds0_2;

        var hits = Physics2D.OverlapCircleAll(position, radius, layer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out HealthBase health))
            {
                health.TakeHit(damage);
            }
        }
    }
}