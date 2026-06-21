using Unity.VisualScripting;
using UnityEngine;

public class DropItems : Interactable
{
    public Creature Creature;

    public override void Interact(Creature creature)
    {
        base.Interact(creature);

        Creature.DropItems();
    }
}