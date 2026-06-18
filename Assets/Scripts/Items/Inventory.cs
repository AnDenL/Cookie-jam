using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;

[Serializable]
public class Inventory
{
    public int maxSlots = 20;
    public List<ItemStack> items = new();
    public event Action<int> OnSlotChange;
    public event Action<ItemStack, int> SlotReinit;
    public event Action<ItemStack, int> OnNewSlot;
    public event Action<Item> OnItemChanged;

    public int AddItem(Item item, int count)
    {
        if (item.Stackable)
        {
            int index = items.FindIndex(stack => stack.Item.Id == item.Id && !stack.IsFull());
            ItemStack existing = null;
            if (index != -1)
                existing = items[index];

            if (existing != null)
            {
                int change = existing.Add(count);

                OnSlotChange?.Invoke(index);
                OnItemChanged?.Invoke(item);
                return change;
            }
        }

        if (items.Count >= maxSlots)
            return 0;

        int added = Math.Min(count, item.MaxStack);
        ItemStack newItemStack = new ItemStack(item, added);
        items.Add(newItemStack);

        OnNewSlot?.Invoke(newItemStack, items.Count - 1);
        OnItemChanged?.Invoke(item);
        return added;
    }

    public bool RemoveItem(Item item, int count)
    {
        int index = items.FindIndex(stack => stack.Item.Id == item.Id);
        ItemStack stack = null;

        if (index != -1)
            stack = items[index];
        else return false;

        if (stack.Count < count)
            return false;

        if (stack.Item.Stackable)
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
            stack.Count = 0;
            OnSlotChange?.Invoke(index);
            items.Remove(stack);
        }
        OnItemChanged?.Invoke(item);
        return true;
    }

    public void RemoveStack(int index)
    {
        ItemStack stack = items[index];
        Item item = stack.Item;

        stack.Count = 0;
        OnSlotChange?.Invoke(index);
        OnItemChanged?.Invoke(item);
        items.Remove(stack);
    }

    public void Swap(int idA, int idB)
    {
        (items[idB], items[idA]) = (items[idA], items[idB]);

        SlotReinit?.Invoke(items[idA], idA);
        SlotReinit?.Invoke(items[idB], idB);
    }

    public ItemStack GetStackById(int id) => items[id];

    public int GetItemCount(Item item)
    {
        return items.Where(s => s.Item.Id == item.Id).Sum(s => s.Count);
    }
}