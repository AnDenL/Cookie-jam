using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Items/Heal", order = -1000)]
public class Heal : Item
{
    public float Amount;
    public AudioClip Sound;

    public override void Use(Creature creature)
    {
        creature.Heal(Amount);
        creature.PlaySound(Sound);
        ParticleManager.PlayParticle("Heal", creature.transform.position, (int)Amount);
    }
}