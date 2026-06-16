using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Items/BuildingItem", order = -1000)]
public class BuildingItem : Item
{
    public GameObject Prefab;
    public AudioClip Sound;
    public float radius = 1f;

    public override bool Use(Creature creature)
    {
        Vector2Int pos = Vector2Int.RoundToInt(Game.mainCamera.ScreenToWorldPoint(Input.mousePosition));

        if (Vector2.Distance(creature.transform.position, pos) > 5) 
        {
            Hints.Show("Too far", 1.5f);
            return false;
        }
        
        if (Physics2D.OverlapCircleAll(pos, radius, LayerMask.GetMask("Nature", "Player", "Enemy", "Default")).Length > 0)
        {
            return false;
        }

        Instantiate(Prefab, (Vector2)pos, Quaternion.identity);

        creature.PlaySound(Sound);
        ParticleManager.PlayParticle("Poof", pos, 10);
        Preview.Instance.enabled = false;
        return true;
    }

    public override void Deselect(Creature creature)
    {
        Preview.Instance.enabled = false;
    }

    public override void Select(Creature creature)
    {
        Preview.Instance.enabled = true;
    }

    public override void WhileSelected(Creature creature)
    {
        Preview.Instance.enabled = true;
        Vector2Int pos = Vector2Int.RoundToInt(Game.mainCamera.ScreenToWorldPoint(Input.mousePosition));

        Preview.Instance.transform.position = (Vector2)pos;

        if (Vector2.Distance(creature.transform.position, pos) > 5) 
        {
            Preview.Instance.color = Color.red;
            return;
        }
        
        if (Physics2D.OverlapCircleAll(pos, radius, LayerMask.GetMask("Nature", "Player", "Enemy", "Default")).Length > 0)
        {
            Preview.Instance.color = Color.red;
            return;
        }

        Preview.Instance.color = Color.white;
    }
}