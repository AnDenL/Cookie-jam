using System.Collections;
using UnityEngine;

public class ItemPickUp : Interactable
{
    public ItemStack stack;
    public ArcAnim anim;

    public void Set(ItemStack stack)
    {
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        this.stack = stack;
        sr.sprite = stack.Item.Icon;
    }

    public override void Interact(Creature creature)
    {
        int itemChange = creature.Inventory.AddItem(stack.Item, stack.Count);

        if (itemChange == stack.Count && canBeInteracted)
            StartCoroutine(PickUpAnim(creature.transform));

        else if (itemChange == 0)
            Hints.Instance.ShowHint("Inventory full, cannot pick up item.", 2);

        else
            stack.Count -= itemChange;
    }

    public void Interact(int itemChange, Transform player)
    {
        if (itemChange == stack.Count && canBeInteracted)
            StartCoroutine(PickUpAnim(player));
        else
            stack.Count -= itemChange;
    }

    public IEnumerator PickUpAnim(Transform target)
    {
        float t = 0;
        key.SetActive(false);
        gameObject.layer = 1 << 0;
        canBeInteracted = false;

        while (t < 1)
        {
            t += Time.deltaTime * 3;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1 - t);
            transform.position = Vector2.MoveTowards(transform.position, target.position, Time.deltaTime * 20 * t);
            yield return null;
        }

        Destroy(gameObject);
    }
}