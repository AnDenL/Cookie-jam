using UnityEngine;

[CreateAssetMenu(fileName = "ApplyEffect", menuName = "Items/ApplyEffect", order = -1000)]
public class ApplyEffect : Item
{
    public Effect effect;
    public bool Dispel;
    public AudioClip Sound;

    public override bool Use(Creature creature)
    {
        creature.AddEffect(effect);
        creature.PlaySound(Sound);
        return true;
    }
}