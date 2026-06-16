using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Hand", order = -1000)]
public class Hand : Item
{
    public float damage;
    public float cooldown;
    public float zoffset;
    public AudioClip sound;

    private float lastAttack;

    public override bool Use(Creature creature)
    {
        if (Time.time < lastAttack + cooldown) return false;

        Vector2 dir = Game.mainCamera.ScreenToWorldPoint(Input.mousePosition) - creature.transform.position;
        var pos = (Vector2)creature.transform.position + dir.normalized;
        var angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

        ParticleManager.PlayParticle("Slash", pos, 1, angle - zoffset);
        
        creature.PlaySound(sound);

        var hits = Physics2D.OverlapCircleAll(pos, 1.5f, LayerMask.GetMask("Nature", "Enemy"));

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out HealthBase health))
            {
                health.TakeHit(1);
            }
        }
        lastAttack = Time.time;
        return true;
    }

    public override void Select(Creature creature)
    {
        lastAttack = Time.time;
    }
}