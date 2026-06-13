using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item", order = -1000)]
public class Item : ScriptableObject
{
    public int id => Name.GetHashCode();
    public string Name;
    public Sprite icon;
    public int maxStack = 99;
    public bool stackable;
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

    public bool IsFull() => Count == Item.maxStack;

    public int Add(int value)
    {
        int previousCount = Count;
        Count = Math.Min(Count + value, Item.maxStack);

        return Count - previousCount;
    }
}