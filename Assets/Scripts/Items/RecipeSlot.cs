using System.Collections.Generic;
using Creatures;
using UnityEngine;
using UnityEngine.Diagnostics;

public class RecipeSlot : MonoBehaviour
{
    public Recipe recipe;

    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private ItemSlot resultSlot;

    private List<ItemSlot> slots = new();

    private bool rebuilded = false;

    private void Start()
    {
        PlayerController.Player.Inventory.OnItemRemove += UpdateUI;
    }

    public void SetRecipe(Recipe newRecipe)
    {
        recipe = newRecipe;

        bool available = true;
        
        foreach (ItemStack stack in recipe.RequiredObjects)
        {
            slots.Add(Instantiate(itemSlotPrefab, transform.GetChild(0)).GetComponent<ItemSlot>());
            slots[^1].SetItem(stack);

            bool hasItem = PlayerController.Player.Inventory.GetItemCount(stack.Item) < stack.Count;
            available = available && hasItem;

            slots[^1].Disabled(hasItem);
        }

        resultSlot.SetItem(recipe.ResultObjects);
        resultSlot.Disabled(available);

        UpdateUI();
    }

    private void OnEnable()
    {
        if (!rebuilded) UpdateUI();
    }

    public void UpdateUI()
    {
        rebuilded = false;

        if (!gameObject.activeInHierarchy) return;

        bool available = true;

        foreach (var slot in slots)
        {
            bool hasItem = PlayerController.Player.Inventory.GetItemCount(slot.itemStack.Item) < slot.itemStack.Count;
            available = available && hasItem;

            slot.Disabled(hasItem);
        }

        resultSlot.Disabled(available);
        rebuilded = true;
    }
}