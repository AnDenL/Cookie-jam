using System.Collections.Generic;
using Creatures;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

public class RecipeSlot : MonoBehaviour
{
    public Recipe recipe;

    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private ItemSlot resultSlot;
    [SerializeField] private Button button;

    private List<ItemSlot> slots = new();

    public bool NeedsRebuild = false;

    public void SetRecipe(Recipe newRecipe)
    {
        recipe = newRecipe;

        bool available = true;
        
        foreach (ItemStack stack in recipe.RequiredObjects)
        {
            slots.Add(Instantiate(itemSlotPrefab, transform.GetChild(0)).GetComponent<ItemSlot>());
            slots[^1].SetItem(stack);

            bool hasItem = PlayerController.Player.Inventory.GetItemCount(stack.Item) >= stack.Count;
            available = available && hasItem;

            slots[^1].Disabled(!hasItem);
        }

        resultSlot.SetItem(recipe.ResultObjects);
        resultSlot.Disabled(!available);

        button.interactable = available;

        UpdateSlot();
    }

    public void Craft()
    {
        foreach (ItemStack stack in recipe.RequiredObjects) 
        {
            if (!PlayerController.Player.Inventory.RemoveItem(stack.Item, stack.Count)) return;
        }
        PlayerController.Player.Inventory.AddItem(recipe.ResultObjects.Item, recipe.ResultObjects.Count);
    }

    private void OnEnable()
    {
        if (NeedsRebuild) UpdateSlot();
    }

    public void UpdateSlot()
    {
        bool available = true;

        foreach (var slot in slots)
        {
            bool hasItem = PlayerController.Player.Inventory.GetItemCount(slot.itemStack.Item) >= slot.itemStack.Count;
            available = available && hasItem;

            slot.Disabled(!hasItem);
        }

        resultSlot.Disabled(!available);
        button.interactable = available;
        NeedsRebuild = false;
    }
}