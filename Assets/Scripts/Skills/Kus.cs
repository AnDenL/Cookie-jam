using System.Collections;
using Creatures;
using UnityEngine;

public class Kus : DirectionSkill
{
    private static readonly WaitForSeconds _waitForSeconds0_2 = new(0.2f);
    public float damage;
    public float radius;
    public string effect;
    public LayerMask layer;

    public override void Activate(Vector2 position)
    {
        var pos = owner.transform.position + (Vector3)position;
        owner.Cast(MakeKus(pos));
        ParticleManager.PlayParticle(effect, pos, 1);
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