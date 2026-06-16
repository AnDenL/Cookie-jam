using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Items/Heal", order = -1000)]
public class Heal : Item
{
    public float Amount;
    public bool Dispel;
    public AudioClip Sound;

    public override bool Use(Creature creature)
    {
        if (creature.HealthComponent.Health == creature.HealthComponent.MaxHealth) return false;
        
        creature.Heal(Amount);
        creature.PlaySound(Sound);
        ParticleManager.PlayParticle("Heal", creature.transform.position, (int)Amount);
        return true;
    }
}