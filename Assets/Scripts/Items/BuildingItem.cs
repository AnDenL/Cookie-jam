using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Items/BuildingItem", order = -1000)]
public class BuildingItem : Item
{
    public GameObject Prefab;
    public AudioClip Sound;

    public override bool Use(Creature creature)
    {
        Vector2Int pos = Vector2Int.RoundToInt(Game.mainCamera.ScreenToWorldPoint(Input.mousePosition));

        if (Vector2.Distance(creature.transform.position, pos) > 5) 
        {
            Hints.Show("Too far", 1.5f);
            return false;
        }
        
        if (Physics2D.OverlapCircleAll(pos, 1f, LayerMask.GetMask("Nature", "Player", "Enemy", "Default")).Length > 0)
        {
            return false;
        }

        Instantiate(Prefab, (Vector2)pos, Quaternion.identity);

        creature.PlaySound(Sound);
        ParticleManager.PlayParticle("Poof", pos, 10);
        return true;
    }
}