using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item", order = -1000)]
public class Item : ScriptableObject
{
    public int Id => Name.GetHashCode();
    public string Name;
    public ItemType Type = ItemType.Material;
    public Sprite Icon;
    public int MaxStack = 99;
    public bool Stackable;
    public bool Consumable;
    public float Light = 0;

    public virtual bool Use(Creature creature) { return false; }
    public virtual void Select(Creature creature) {}
    public virtual void WhileSelected(Creature creature) {}
    public virtual void Deselect(Creature creature) {}
}

public enum ItemType
{
    Material,
    Active
}

[Serializable]
public class ItemStack
{
    public Item Item;
    public int Count;

    public ItemStack(Item item, int count)
    {
        Item = item;
        Count = count;
    }

    public bool IsFull() => Count == Item.MaxStack;

    public int Add(int value)
    {
        int previousCount = Count;
        Count = Math.Min(Count + value, Item.MaxStack);

        return Count - previousCount;
    }
}