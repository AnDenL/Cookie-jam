using Creatures;
using UnityEngine;

[CreateAssetMenu(fileName = "Soul", menuName = "Items/Soul", order = -1000)]
public class Soul : Item
{
    public AIController controller;
    public AudioClip Sound;

    public override bool Use(Creature creature)
    {
        Vector2 pos = Game.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        var hits = Physics2D.OverlapCircleAll(pos, 2f, LayerMask.GetMask("Nature"));
        GameObject previewPos = null;
        float dist = 5;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Trans")) continue;

            if (Vector2.Distance(creature.transform.position, hit.transform.position) < dist)
            {
                previewPos = hit.gameObject;
            }
        }
        
        creature.PlaySound(Sound);
        //ParticleManager.PlayParticle("Heal", creature.transform.position, (int)Amount);
        return true;
    }

    public override void WhileSelected(Creature creature)
    {
        Vector2 pos = Game.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        var hits = Physics2D.OverlapCircleAll(pos, 2f, LayerMask.GetMask("Nature"));
        Vector2? previewPos = null;
        float dist = 5;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Trans")) continue;

            if (Vector2.Distance(creature.transform.position, hit.transform.position) < dist)
            {
                previewPos = hit.transform.position;
            }
        }

        if (previewPos != null)
        {
            Preview.Instance.enabled = true;
            Preview.Instance.transform.position = (Vector2)previewPos;
        }
        else Preview.Instance.enabled = false;
    }
}