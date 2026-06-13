using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeathLoot : MonoBehaviour
{
    public ItemDrop[] drop;
    [SerializeField] private GameObject itemPrefab;

    private void Start()
    {
        GetComponent<HealthBase>().OnDeath += SpawnItems;
    }

    public void SpawnItems()
    {
        List<ItemStack> itemsToSpawn = new();

        foreach (var item in drop)
            if (item.chance > Random.Range(0, 100))
                itemsToSpawn.Add(item.itemStack);

        foreach (var itemToSpawn in itemsToSpawn)
            SpawnItem(itemToSpawn);
    }

    private void SpawnItem(ItemStack stack)
    {
        var item = Instantiate(itemPrefab, transform.position, Quaternion.identity).GetComponent<ItemPickUp>();
        item.Set(stack);
        item.anim.DropTo((Vector2)transform.position + Random.insideUnitCircle);
    }
}

[Serializable]
public class ItemDrop
{
    public ItemStack itemStack;
    public float chance;
}
