using UnityEngine;

[CreateAssetMenu(fileName = "SpeedModifier", menuName = "Game/Effects/SpeedModifier")]
public class SpeedModifier : Effect
{
    public override bool IsPositive() => Strenght > 1;

    public override void OnApply()
    {
        owner.Speed *= Strenght;
    }

    public override void OnRemove()
    {
        owner.Speed /= Strenght;
    }
}