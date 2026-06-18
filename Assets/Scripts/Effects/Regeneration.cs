using UnityEngine;

[CreateAssetMenu(fileName = "Regeneration", menuName = "Game/Effects/Regeneration")]
public class Regeneration : Effect
{
    public override bool IsPositive() => true;

    public override void OnApply()
    {
        
    }

    public override void Tick(float dt)
    {
        base.Tick(dt);
        owner.HealthComponent.HealthEditable += Strenght * dt;
    }

    public override void OnRemove()
    {
        
    }
}