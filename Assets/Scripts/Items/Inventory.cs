using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Inventory
{
    static public Inventory Instance;

    public int maxSlots = 20;
    public List<ItemStack> items = new();
    public event Action<int> OnSlotChange;
    public event Action<ItemStack, int> OnNewSlot;
    public event Action OnInventoryChange;

    public Inventory() => Instance = this;

    public int AddItem(Item item, int count = 1)
    {
        if (item.stackable)
        {
            int index = items.FindIndex(stack => stack.Item.id == item.id && !stack.IsFull());
            ItemStack existing = null;
            if (index != -1)
                existing = items[index];

            if (existing != null)
            {
                int change = existing.Add(count);

                OnSlotChange?.Invoke(index);
                OnInventoryChange?.Invoke();
                return change;
            }
        }

        if (items.Count >= maxSlots)
            return 0;

        int added = Math.Min(count, item.maxStack);
        ItemStack newItemStack = new ItemStack(item, added);
        items.Add(newItemStack);

        OnNewSlot?.Invoke(newItemStack, items.Count - 1);
        OnInventoryChange?.Invoke();
        return added;
    }

    public bool RemoveItem(Item item, int count = 1)
    {
        int index = items.FindIndex(stack => stack.Item.id == item.id);
        ItemStack stack = null;

        if (index != -1)
            stack = items[index];
        else return false;

        if (stack.Count < count)
            return false;

        if (stack.Item.stackable)
        {
            stack.Count -= count;
            if (stack.Count <= 0)
            {
                items.Remove(stack);
            }
            OnSlotChange?.Invoke(index);
        }
        else
        {
            items.Remove(stack);
        }
        OnInventoryChange?.Invoke();
        return true;
    }

    public int GetItemCount(Item item)
    {
        return items.Where(s => s.Item.id == item.id).Sum(s => s.Count);
    }
}
